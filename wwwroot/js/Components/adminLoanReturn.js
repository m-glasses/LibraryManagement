document.addEventListener("DOMContentLoaded", () => {

    const modalElement =
        document.getElementById("confirmReturnModal");

    if (!modalElement) {
        return;
    }

    const loanIdInput =
        modalElement.querySelector('input[name="LoanId"]');

    modalElement.addEventListener("show.bs.modal", (event) => {

        const trigger = event.relatedTarget;

        if (!trigger) {
            return;
        }

        const loanId =
            trigger.dataset.loanId;

        if (loanIdInput) {
            loanIdInput.value = loanId;
        }
    });

    modalElement.addEventListener("shown.bs.modal", () => {

        jalaliDatepicker.startWatch({
    targetValueInput: "attr",
    targetValueType: "attr",
    zIndex: 1060
    });

    });

});