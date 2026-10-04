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

        const rawValue =
            amountInput.value.replace(/,/g, "");

        const amount =
            Number(rawValue);

        if (!amount || amount < 1000 || amount > 100000000) {
            event.preventDefault();

            amountInput.classList.add("is-invalid");
            return;
        }

        amountInput.classList.remove("is-invalid");

        // Send the raw numeric value to the server.
        amountInput.value = rawValue;
    });

    amountInput.addEventListener("input", () => {

        const value =
            amountInput.value.replace(/\D/g, "");

        if (!value) {
            amountInput.value = "";
            return;
        }

        amountInput.value =
            Number(value).toLocaleString("en-US");
    });

});

