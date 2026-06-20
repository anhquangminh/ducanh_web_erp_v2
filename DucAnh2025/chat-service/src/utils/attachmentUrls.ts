import path from 'path';

export function absoluteAttachmentUrl(baseUrl: string, url: string) {
  if (!url) return '';
  if (/^https?:\/\//i.test(url)) return url;
  const normalized = url.startsWith('/') ? url : `/${url}`;
  return `${baseUrl.replace(/\/$/, '')}${normalized}`;
}

export function attachmentDownloadUrl(baseUrl: string, url: string, fileName?: string) {
  const storedFileName = path.basename(new URL(absoluteAttachmentUrl(baseUrl, url)).pathname);
  const downloadUrl = `${baseUrl.replace(/\/$/, '')}/api/chat/uploads/${encodeURIComponent(storedFileName)}/download`;
  return fileName
    ? `${downloadUrl}?name=${encodeURIComponent(fileName)}`
    : downloadUrl;
}

export function enrichAttachmentUrls(baseUrl: string, attachment: any) {
  const url = String(attachment?.url ?? '');
  const fullUrl = absoluteAttachmentUrl(baseUrl, url);

  return {
    ...attachment,
    url,
    absoluteUrl: fullUrl,
    previewUrl: ['image', 'video', 'voice'].includes(String(attachment?.type)) ? fullUrl : '',
    downloadUrl: url ? attachmentDownloadUrl(baseUrl, url, attachment?.fileName) : ''
  };
}

export function enrichMessageAttachmentUrls(baseUrl: string, message: any) {
  if (!message || !baseUrl) return message;
  return {
    ...message,
    attachments: (message.attachments ?? []).map((attachment: any) => enrichAttachmentUrls(baseUrl, attachment)),
    replyTo: message.replyTo
      ? {
          ...message.replyTo,
          attachments: (message.replyTo.attachments ?? []).map((attachment: any) => enrichAttachmentUrls(baseUrl, attachment))
        }
      : message.replyTo
  };
}
