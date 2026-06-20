import { env } from '../config/env.ts';

type AiChatMessage = {
  role: 'user' | 'assistant' | 'system';
  content: string;
};

type AiGenerateInput = {
  messages: AiChatMessage[];
};

function compactText(value: unknown) {
  return String(value ?? '').trim();
}

function extractOpenAiText(data: any) {
  const outputText = compactText(data?.output_text);
  if (outputText) return outputText;

  const chunks: string[] = [];
  for (const item of data?.output ?? []) {
    for (const content of item?.content ?? []) {
      const text = compactText(content?.text);
      if (text) chunks.push(text);
    }
  }
  return chunks.join('\n').trim();
}

function extractGeminiText(data: any) {
  return (data?.candidates ?? [])
    .flatMap((candidate: any) => candidate?.content?.parts ?? [])
    .map((part: any) => compactText(part?.text))
    .filter(Boolean)
    .join('\n')
    .trim();
}

async function postJson(url: string, body: unknown, headers: Record<string, string>) {
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), env.AI_REQUEST_TIMEOUT_MS);

  try {
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...headers,
      },
      body: JSON.stringify(body),
      signal: controller.signal,
    });
    const data = await response.json().catch(() => null);
    if (!response.ok) {
      const message = compactText(data?.error?.message || data?.message) || `AI provider failed with status ${response.status}.`;
      throw new Error(message);
    }
    return data;
  } finally {
    clearTimeout(timeout);
  }
}

export class AiService {
  isEnabled() {
    return Boolean(env.AI_PROVIDER);
  }

  providerName() {
    return env.AI_PROVIDER ?? null;
  }

  async generateReply(input: AiGenerateInput) {
    if (!env.AI_PROVIDER) throw new Error('AI provider is not configured.');

    switch (env.AI_PROVIDER) {
      case 'openai':
        return this.generateOpenAiReply(input.messages);
      case 'gemini':
        return this.generateGeminiReply(input.messages);
      case 'ollama':
        return this.generateOllamaReply(input.messages);
      default:
        throw new Error('Unsupported AI provider.');
    }
  }

  private async generateOpenAiReply(messages: AiChatMessage[]) {
    if (!env.OPENAI_API_KEY) throw new Error('OPENAI_API_KEY is required for AI_PROVIDER=openai.');
    const data = await postJson(
      `${env.OPENAI_BASE_URL.replace(/\/$/, '')}/responses`,
      {
        model: env.OPENAI_MODEL,
        instructions: env.AI_SYSTEM_PROMPT,
        input: messages
          .filter((message) => message.role !== 'system')
          .map((message) => ({
            role: message.role,
            content: message.content,
          })),
      },
      { Authorization: `Bearer ${env.OPENAI_API_KEY}` },
    );
    const text = extractOpenAiText(data);
    if (!text) throw new Error('OpenAI returned an empty response.');
    return text;
  }

  private async generateGeminiReply(messages: AiChatMessage[]) {
    if (!env.GEMINI_API_KEY) throw new Error('GEMINI_API_KEY is required for AI_PROVIDER=gemini.');
    const contents = messages
      .filter((message) => message.role !== 'system')
      .map((message) => ({
        role: message.role === 'assistant' ? 'model' : 'user',
        parts: [{ text: message.content }],
      }));
    const data = await postJson(
      `${env.GEMINI_BASE_URL.replace(/\/$/, '')}/models/${encodeURIComponent(env.GEMINI_MODEL)}:generateContent`,
      {
        systemInstruction: {
          parts: [{ text: env.AI_SYSTEM_PROMPT }],
        },
        contents,
      },
      { 'X-goog-api-key': env.GEMINI_API_KEY },
    );
    const text = extractGeminiText(data);
    if (!text) throw new Error('Gemini returned an empty response.');
    return text;
  }

  private async generateOllamaReply(messages: AiChatMessage[]) {
    const data = await postJson(
      `${env.OLLAMA_BASE_URL.replace(/\/$/, '')}/api/chat`,
      {
        model: env.OLLAMA_MODEL,
        stream: false,
        messages: [
          { role: 'system', content: env.AI_SYSTEM_PROMPT },
          ...messages,
        ],
      },
      {},
    );
    const text = compactText(data?.message?.content);
    if (!text) throw new Error('Ollama returned an empty response.');
    return text;
  }
}
