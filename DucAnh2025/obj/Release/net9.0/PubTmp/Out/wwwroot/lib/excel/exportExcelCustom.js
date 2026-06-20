//async function exportExcelCustom(idTable, headers, sheetName = "Sheet1", fileName = "export.xlsx") {
//    const table = document.getElementById(idTable);
//    if (!table) {
//        alert(`Không tìm thấy bảng #${idTable}`);
//        return;
//    }

//    const totalCols = table.querySelectorAll('thead tr:last-child th, thead tr:last-child td').length;

//    const workbook = new ExcelJS.Workbook();
//    workbook.creator = 'Ứng dụng';
//    const ws = workbook.addWorksheet(sheetName);

//    // helper style
//    function setCellStyle(cell, { fontSize = 11, bold = false, align = 'left', wrap = true, border = false } = {}) {
//        cell.font = { name: "Times New Roman", size: fontSize, bold: bold };
//        cell.alignment = { horizontal: align, vertical: "middle", wrapText: wrap };
//        if (border) {
//            cell.border = {
//                top: { style: 'thin' }, left: { style: 'thin' },
//                bottom: { style: 'thin' }, right: { style: 'thin' }
//            };
//        }
//    }

//    // render headers
//    let currentRow = 1;
//    headers.forEach(h => {
//        const cols = h.cols || totalCols;
//        const cell = ws.getCell(currentRow, 1);
//        cell.value = h.text;
//        ws.mergeCells(currentRow, 1, currentRow, cols);
//        setCellStyle(cell, { fontSize: h.fontSize, bold: h.bold, align: h.align, border: false });
//        ws.getRow(currentRow).height = 18;
//        currentRow++;
//    });

//    // table header
//    const headerCells = Array.from(table.querySelectorAll('thead tr:last-child th, thead tr:last-child td'));
//    const rowHeader = ws.getRow(currentRow);
//    headerCells.forEach((th, i) => {
//        const cell = rowHeader.getCell(i + 1);
//        cell.value = th.innerText.trim();
//        setCellStyle(cell, { fontSize: 11, bold: true, align: "center", border: true });
//    });
//    currentRow++;

//    // table body
//    table.querySelectorAll('tbody tr').forEach(tr => {
//        const row = ws.getRow(currentRow);
//        tr.querySelectorAll('td, th').forEach((td, i) => {
//            const val = td.innerText.trim();
//            const num = parseFloat(val.replace(/,/g, ''));
//            const cell = row.getCell(i + 1);
//            cell.value = (!isNaN(num) && val !== '') ? num : val;
//            setCellStyle(cell, { fontSize: 11, align: "left", border: true });
//        });
//        currentRow++;
//    });

//    // set column width
//    for (let c = 1; c <= totalCols; c++) {
//        ws.getColumn(c).width = 15;
//    }

//    // export
//    const buffer = await workbook.xlsx.writeBuffer();
//    saveAs(new Blob([buffer], { type: "application/octet-stream" }), fileName);
//}


async function exportExcelCustom(idTable, headers, sheetName = "Sheet1", fileName = "export.xlsx", excludeColumns = []) {
    const table = document.getElementById(idTable);
    if (!table) {
        alert(`Không tìm thấy bảng #${idTable}`);
        return;
    }

    // Lấy số cột thực tế (không tính cột loại bỏ)
    const headerCells = Array.from(table.querySelectorAll('thead tr:last-child th, thead tr:last-child td'));
    const totalCols = headerCells.length - excludeColumns.length;

    const workbook = new ExcelJS.Workbook();
    workbook.creator = 'Ứng dụng';
    const ws = workbook.addWorksheet(sheetName);

    // helper style
    function setCellStyle(cell, { fontSize = 11, bold = false, align = 'left', wrap = true, border = false } = {}) {
        cell.font = { name: "Times New Roman", size: fontSize, bold: bold };
        cell.alignment = { horizontal: align, vertical: "middle", wrapText: wrap };
        if (border) {
            cell.border = {
                top: { style: 'thin' }, left: { style: 'thin' },
                bottom: { style: 'thin' }, right: { style: 'thin' }
            };
        }
    }

    // render headers
    let currentRow = 1;
    headers.forEach(h => {
        // Nếu truyền cols thì trừ đi số cột loại bỏ nếu cần
        let cols = h.cols || totalCols;
        if (cols > totalCols) cols = totalCols;
        const cell = ws.getCell(currentRow, 1);
        cell.value = h.text;
        ws.mergeCells(currentRow, 1, currentRow, cols);
        setCellStyle(cell, { fontSize: h.fontSize, bold: h.bold, align: h.align, border: false });
        ws.getRow(currentRow).height = 18;
        currentRow++;
    });

    // table header
    const rowHeader = ws.getRow(currentRow);
    headerCells.forEach((th, i) => {
        if (excludeColumns.includes(i)) return;
        const cell = rowHeader.getCell(rowHeader.cellCount + 1);
        cell.value = th.innerText.trim();
        setCellStyle(cell, { fontSize: 11, bold: true, align: "center", border: true });
    });
    currentRow++;

    // table body
    table.querySelectorAll('tbody tr').forEach(tr => {
        const row = ws.getRow(currentRow);
        let cellIndex = 1;
        tr.querySelectorAll('td, th').forEach((td, i) => {
            if (excludeColumns.includes(i)) return;
            const val = td.innerText.trim();
            const num = parseFloat(val.replace(/,/g, ''));
            const cell = row.getCell(cellIndex++);
            cell.value = (!isNaN(num) && val !== '') ? num : val;
            setCellStyle(cell, { fontSize: 11, align: "left", border: true });
        });
        currentRow++;
    });

    // set column width
    for (let c = 1; c <= totalCols; c++) {
        ws.getColumn(c).width = 15;
    }

    // export
    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer], { type: "application/octet-stream" }), fileName);
}

function copyTable(idTable) {
    var range = document.createRange();
    range.selectNode(document.getElementById(idTable));
    window.getSelection().removeAllRanges();
    window.getSelection().addRange(range);
    try {
        var successful = document.execCommand('copy');
        alert(successful ? 'Đã copy vào clipboard!' : 'Copy thất bại!');
    } catch (err) {
        alert('Trình duyệt không hỗ trợ copy bằng document.execCommand.\nBạn có thể thử thủ công (Ctrl+C).');
    }
    window.getSelection().removeAllRanges();
}
