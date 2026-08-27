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

    async function nazivExists(naziv)
    {
        const url = "/Predmet/ProveriNaziv?naziv=" + encodeURIComponent(naziv);
        // If calling API directly, use your API base URL instead
        const response = await fetch(url);
        if (!response.ok) return false;
        const data = await response.json();
        return data.exists === true;
    }

    function validateId(input)
    {
        const value = (input.value || "").trim();
        if (!value) {
            showError(input, "Id predmeta je obavezan");
            return false;
        }
        if (value.length !== 5) {
            showError(input, "ID predmeta mora imati tačno 5 karaktera");
            return false;
        }
        clearError(input);
        return true;
    }

    async function validateNaziv(input)
    {
        const value = (input.value || "").trim();
        if (!value) {
            showError(input, "Naziv predmeta je obavezan");
            return false;
        }

        try {
            const exists = await nazivExists(value);
            if (exists) {
                showError(input, "Predmet sa ovim nazivom već postoji");
                return false;
            }
        } catch {
            // network error – don’t block hard, or show a message
            showError(input, "Greška pri proveri naziva");
            return false;
        }

        clearError(input);
        return true;
    }

    document.addEventListener("DOMContentLoaded", function ()
    {
        const form = document.getElementById("predmetForm");
        if (!form) return;

        const idInput = form.querySelector('[name="ID"]');
        const nazivInput = form.querySelector('[name="NazivPredmeta"]');

        idInput?.addEventListener("blur", () => validateId(idInput));
        nazivInput?.addEventListener("blur", () => validateNaziv(nazivInput));

        form.addEventListener("submit", async function (e)
        {
            e.preventDefault();

            const idOk = validateId(idInput);
            const nazivOk = await validateNaziv(nazivInput);

            if (idOk && nazivOk) {
                form.submit();
            }
        });
    });
})();