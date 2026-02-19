let sortColumn = null;
let sortDirection = 'asc';

document.addEventListener('DOMContentLoaded', () => {
    const table = document.getElementById('contactsTable');
    const searchInput = document.getElementById('searchInput');

    if (!table) return;

    const headers = table.querySelectorAll('thead th');

    headers.forEach((header, index) => {
        header.addEventListener('click', () => sortTable(index));
    });

    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            filterTable(e.target.value);
        });
    }
});

const sortTable = (columnIndex) => {
    const table = document.getElementById('contactsTable');
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));

    sortDirection = (sortColumn === columnIndex && sortDirection === 'asc')
        ? 'desc'
        : 'asc';

    sortColumn = columnIndex;

    rows.sort((rowA, rowB) => {
        const a = rowA.children[columnIndex].textContent.trim();
        const b = rowB.children[columnIndex].textContent.trim();

        return sortDirection === 'asc'
            ? a.localeCompare(b, undefined, { numeric: true })
            : b.localeCompare(a, undefined, { numeric: true });
    });

    rows.forEach(row => tbody.appendChild(row));

    updateIndicators();
}

const filterTable = (searchTerm) => {
    const rows = document.querySelectorAll('#contactsTable tbody tr');
    const term = searchTerm.toLowerCase();

    rows.forEach(row => {
        const text = row.innerText.toLowerCase();
        row.style.display = text.includes(term) ? '' : 'none';
    });
}

const updateIndicators = () => {
    const headers = document.querySelectorAll('#contactsTable thead th');

    headers.forEach((header, index) => {
        const indicator = header.querySelector('.sort-indicator');
        if (!indicator) return;

        indicator.textContent =
            index === sortColumn
                ? (sortDirection === 'asc' ? '▲' : '▼')
                : '';
    });
}
