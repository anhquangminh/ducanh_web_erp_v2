//function formatDate(dateStr) {
//    if (!dateStr) return "";
//    return new Date(dateStr).toISOString().split("T")[0];
//}
function formatDate(dateStr) {
    if (!dateStr) return "";

    const d = new Date(dateStr);

    const year = d.getFullYear();
    const month = ("0" + (d.getMonth() + 1)).slice(-2);
    const day = ("0" + d.getDate()).slice(-2);

    return `${year}-${month}-${day}`;
}

function formatTime(dateString) {
    if (!dateString) return "";
    const d = new Date(dateString);
    const h = ("0" + d.getHours()).slice(-2);
    const m = ("0" + d.getMinutes()).slice(-2);
    return `${h}:${m}`;
}

function formatVNDate(dateStr) {
    if (!dateStr) return "—";
    const d = new Date(dateStr);
    const day = ("0" + d.getDate()).slice(-2);
    const month = ("0" + (d.getMonth() + 1)).slice(-2);
    const year = d.getFullYear();
    return `${day}/${month}/${year}`;
}

function parseVNDate(dateStr) {
    if (!dateStr) return null;

    const parts = dateStr.split("/");
    if (parts.length !== 3) return null;

    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1;
    const year = parseInt(parts[2], 10);

    return new Date(year, month, day);
}