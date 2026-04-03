// أدوات التصدير والطباعة - تصدير الجداول إلى Excel وطباعة المحتوى
(function () {
    'use strict';

    // تصدير الجدول إلى Excel (CSV مع دعم Unicode للعربية)
    window.exportTableToExcel = function (tableSelector, fileName) {
        var table = document.querySelector(tableSelector);
        if (!table) { alert('لا يوجد جدول للتصدير'); return; }

        var rows = table.querySelectorAll('tr');
        var csvContent = '\uFEFF'; // BOM for UTF-8

        rows.forEach(function (row) {
            var cols = row.querySelectorAll('td, th');
            var rowData = [];
            cols.forEach(function (col) {
                // تجاهل عمود الإجراءات
                if (col.querySelector('.action-buttons, .btn-action')) return;
                var text = col.innerText.replace(/"/g, '""').trim();
                rowData.push('"' + text + '"');
            });
            if (rowData.length > 0) csvContent += rowData.join(',') + '\n';
        });

        var blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        var link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = (fileName || 'export') + '_' + new Date().toISOString().slice(0, 10) + '.csv';
        link.click();
        URL.revokeObjectURL(link.href);
    };

    // طباعة الجدول
    window.printTable = function (tableSelector, title) {
        var table = document.querySelector(tableSelector);
        if (!table) { alert('لا يوجد جدول للطباعة'); return; }

        // إنشاء نسخة من الجدول بدون عمود الإجراءات
        var clone = table.cloneNode(true);
        clone.querySelectorAll('.action-buttons, .btn-action').forEach(function (el) {
            el.closest('td, th')?.remove();
        });
        // حذف عمود الإجراءات من الهيدر أيضاً
        var headers = clone.querySelectorAll('thead th');
        headers.forEach(function (th) {
            if (th.textContent.trim() === 'الإجراءات') th.remove();
        });

        var printWindow = window.open('', '_blank');
        printWindow.document.write('<!DOCTYPE html><html lang="ar" dir="rtl"><head><meta charset="utf-8">');
        printWindow.document.write('<title>' + (title || 'طباعة') + '</title>');
        printWindow.document.write('<style>');
        printWindow.document.write('body { font-family: "Cairo", "Segoe UI", Tahoma, sans-serif; direction: rtl; padding: 20px; }');
        printWindow.document.write('h2 { text-align: center; margin-bottom: 20px; color: #333; }');
        printWindow.document.write('.print-date { text-align: center; color: #666; margin-bottom: 15px; font-size: 14px; }');
        printWindow.document.write('table { width: 100%; border-collapse: collapse; }');
        printWindow.document.write('th, td { border: 1px solid #ddd; padding: 8px 12px; text-align: right; font-size: 13px; }');
        printWindow.document.write('th { background-color: #f8f9fa; font-weight: 600; }');
        printWindow.document.write('tr:nth-child(even) { background-color: #f9f9f9; }');
        printWindow.document.write('@media print { body { padding: 0; } }');
        printWindow.document.write('</style></head><body>');
        printWindow.document.write('<h2>' + (title || 'تقرير') + '</h2>');
        printWindow.document.write('<div class="print-date">تاريخ الطباعة: ' + new Date().toLocaleDateString('ar-IQ') + '</div>');
        printWindow.document.write(clone.outerHTML);
        printWindow.document.write('</body></html>');
        printWindow.document.close();
        printWindow.focus();
        setTimeout(function () { printWindow.print(); }, 300);
    };
})();
