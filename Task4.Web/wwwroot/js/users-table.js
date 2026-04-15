const selectAll = document.getElementById("selectAll");
const checkboxes = document.querySelectorAll(".row-checkbox");
const buttons = document.querySelectorAll("button[formaction]");

function updateButtons() {
    const anyChecked = Array.from(checkboxes).some(cb => cb.checked);

    buttons.forEach(btn => btn.disabled = !anyChecked);
}

selectAll.addEventListener("change", () => {
    checkboxes.forEach(cb => cb.checked = selectAll.checked);
    updateButtons();
});

checkboxes.forEach(cb => {
    cb.addEventListener("change", updateButtons);
});