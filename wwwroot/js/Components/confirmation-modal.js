document.addEventListener("DOMContentLoaded", () => {

    const modalElement = document.getElementById("confirmationModal");

    if (!modalElement) {
        return;
    }

    const messageElement =
        document.getElementById("confirmationModalMessage");

    const confirmButton =
        document.getElementById("confirmationModalConfirm");

    const modal =
        new bootstrap.Modal(modalElement);

    let currentForm = null;

    document.addEventListener("click", (event) => {

        const trigger =
            event.target.closest("[data-confirm]");

        if (!trigger) {
            return;
        }

        const form =
            trigger.closest("form");

        if (!form) {
            return;
        }

        event.preventDefault();

        currentForm = form;

        const message =
            trigger.dataset.confirm;

        messageElement.textContent =
            message || "آیا از انجام این عملیات مطمئن هستید؟";

        modal.show();
    });

    confirmButton.addEventListener("click", () => {

        if (!currentForm) {
            return;
        }

        currentForm.submit();

        currentForm = null;

        modal.hide();
    });

    modalElement.addEventListener("hidden.bs.modal", () => {
        currentForm = null;
    });
});