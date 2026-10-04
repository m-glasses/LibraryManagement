document.addEventListener("DOMContentLoaded", () => {

    const modalElement =
        document.getElementById("walletDepositModal");

    if (!modalElement) {
        return;
    }

    const form =
        modalElement.querySelector("form");

    const amountInput =
        modalElement.querySelector('input[name="Amount"]');

    if (!form || !amountInput) {
        return;
    }

    form.addEventListener("submit", (event) => {

        const amount = Number(amountInput.value);

        if (!amount || amount < 1000) {
            event.preventDefault();

            amountInput.classList.add("is-invalid");
            return;
        }

        amountInput.classList.remove("is-invalid");
    });

    amountInput.addEventListener("input", () => {

    const value = amountInput.value.replace(/\D/g, "");

    if (!value) {
        amountInput.value = "";
        return;
    }

    amountInput.value =
        Number(value).toLocaleString("en-US");
});

});