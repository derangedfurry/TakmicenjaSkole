(function ()
{
    function toDateTimeLocalValue(date)
    {
        const pad = (n) => n.toString().padStart(2, "0");
        return date.getFullYear() + "-" +
            pad(date.getMonth() + 1) + "-" +
            pad(date.getDate()) + "T" +
            pad(date.getHours()) + ":" +
            pad(date.getMinutes());
    }

    function showError(input, message)
    {
        input.classList.add("is-invalid");
        let span = input.parentElement.querySelector(".js-validation-error");
        if (!span) {
            span = document.createElement("span");
            span.className = "text-danger js-validation-error";
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

    function validateForm(form)
    {
        let valid = true;

        // Required text / select fields (master)
        const requiredSelectors = [
            '[name="Takmicenje.NazivTakmicenja"]',
            '[name="Takmicenje.NazivPredmetaTakmicenja"]',
            '[name="Takmicenje.LokacijaTakmicenja"]',
            '[name="Takmicenje.TipTakmicenja"]',
            '[name="Takmicenje.DatumTakmicenja"]'
        ];

        requiredSelectors.forEach(sel =>
        {
            const el = form.querySelector(sel);
            if (!el) return;
            if (!el.value || !el.value.trim()) {
                showError(el, "Ovo polje je obavezno.");
                valid = false;
            } else {
                clearError(el);
            }
        });

        // Date not in the future
        const dateInput = form.querySelector('[name="Takmicenje.DatumTakmicenja"]');
        if (dateInput && dateInput.value) {
            const selected = new Date(dateInput.value);
            const now = new Date();
            if (selected > now) {
                showError(dateInput, "Datum takmičenja ne može biti u budućnosti.");
                valid = false;
            }
        }

        // Učenici rows
        const rows = form.querySelectorAll("#uceniciBody tr");
        if (rows.length === 0) {
            alert("Dodajte bar jednog učenika.");
            valid = false;
        }

        rows.forEach(row =>
        {
            const sifra = row.querySelector('input[name*="SifraUcenika"]');
            const ime = row.querySelector('input[name*=".Ime"]');
            const prezime = row.querySelector('input[name*="Prezime"]');
            const bodovi = row.querySelector('input[name*="BrojBodova"]');

            if (sifra) {
                if (!sifra.value || sifra.value.length !== 5) {
                    showError(sifra, "Šifra mora imati tačno 5 karaktera.");
                    valid = false;
                } else clearError(sifra);
            }
            if (ime && !ime.value.trim()) {
                showError(ime, "Ime je obavezno.");
                valid = false;
            } else if (ime) clearError(ime);

            if (prezime && !prezime.value.trim()) {
                showError(prezime, "Prezime je obavezno.");
                valid = false;
            } else if (prezime) clearError(prezime);

            if (bodovi) {
                const n = Number(bodovi.value);
                if (bodovi.value === "" || isNaN(n) || n < 0) {
                    showError(bodovi, "Broj bodova mora biti 0 ili više.");
                    valid = false;
                } else clearError(bodovi);
            }
        });

        return valid;
    }

    document.addEventListener("DOMContentLoaded", function ()
    {
        const form = document.getElementById("masterDetailForm");
        if (!form) return;

        // Max datetime-local = now
        const dateInput = form.querySelector('[name="Takmicenje.DatumTakmicenja"]');
        if (dateInput) {
            dateInput.max = toDateTimeLocalValue(new Date());
        }

        form.addEventListener("submit", function (e)
        {
            if (!validateForm(form)) {
                e.preventDefault();
                e.stopPropagation();
            }
        });
    });
})();