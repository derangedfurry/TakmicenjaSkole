(function ()
{
    function showError(input, message)
    {
        input.classList.add("is-invalid");
        let span = input.parentElement.querySelector(".js-validation-error");
        if (!span) {
            span = document.createElement("span");
            span.className = "text-danger js-validation-error d-block";
            input.parentElement.appendChild(span);
        }
        span.textContent = message;
    }

    function clearError(input)
    {
        input.classList.remove("is-invalid");
        const span = input.parentElement.querySelector(".js-validation-error");
        if (span) span.textContent = "";
    }

    async function nazivExists(naziv, excludeId)
    {
        let url = "/Predmet/ProveriNaziv?naziv=" + encodeURIComponent(naziv);
        if (excludeId) {
            url += "&excludeId=" + encodeURIComponent(excludeId);
        }
        const response = await fetch(url);
        if (!response.ok) return false;
        const data = await response.json();
        return data.exists === true;
    }

    async function validateNaziv(input, excludeId)
    {
        const value = (input.value || "").trim();
        if (!value) {
            showError(input, "Naziv predmeta je obavezan");
            return false;
        }

        try {
            const exists = await nazivExists(value, excludeId);
            if (exists) {
                showError(input, "Predmet sa ovim nazivom već postoji");
                return false;
            }
        } catch {
            showError(input, "Greška pri proveri naziva");
            return false;
        }

        clearError(input);
        return true;
    }

    document.addEventListener("DOMContentLoaded", function ()
    {
        const form = document.getElementById("predmetEditForm");
        if (!form) return;

        const idInput = form.querySelector('[name="ID"]');
        const nazivInput = form.querySelector('[name="NazivPredmeta"]');
        const excludeId = idInput ? idInput.value : null;

        nazivInput?.addEventListener("blur", () => validateNaziv(nazivInput, excludeId));

        form.addEventListener("submit", async function (e)
        {
            e.preventDefault();

            const nazivOk = await validateNaziv(nazivInput, excludeId);
            if (nazivOk) {
                form.submit();
            }
        });
    });
})();