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

    async function sifraExists(sifra)
    {
        // Via presentation action that proxies the API, or call API base URL
        const url = "/Ucenik/ProveriSifru?sifra=" + encodeURIComponent(sifra);
        const response = await fetch(url);
        if (!response.ok) return false;
        const data = await response.json();
        return data.exists === true;
    }

    function validateSifraFormat(input)
    {
        const value = (input.value || "").trim();
        if (!value) {
            showError(input, "Šifra učenika je obavezna");
            return false;
        }
        if (value.length !== 5) {
            showError(input, "Šifra učenika mora imati tačno 5 karaktera.");
            return false;
        }
        clearError(input);
        return true;
    }

    async function validateSifra(input)
    {
        if (!validateSifraFormat(input)) return false;

        try {
            const exists = await sifraExists(input.value.trim());
            if (exists) {
                showError(input, "Učenik sa ovom šifrom već postoji");
                return false;
            }
        } catch {
            showError(input, "Greška pri proveri šifre");
            return false;
        }

        clearError(input);
        return true;
    }

    function validateRequired(input, message)
    {
        if (!(input.value || "").trim()) {
            showError(input, message);
            return false;
        }
        clearError(input);
        return true;
    }

    function validateBodovi(input)
    {
        if (input.value === "") {
            showError(input, "Broj bodova je obavezan");
            return false;
        }
        const n = Number(input.value);
        if (isNaN(n) || n < 0 || n > 100) {
            showError(input, "Broj bodova mora biti između 0 i 100.");
            return false;
        }
        clearError(input);
        return true;
    }

    function validateTakmicenje(input)
    {
        if (!input.value || input.value === "0") {
            showError(input, "Izaberite takmičenje");
            return false;
        }
        clearError(input);
        return true;
    }

    document.addEventListener("DOMContentLoaded", function ()
    {
        const form = document.getElementById("ucenikForm");
        if (!form) return;

        const sifra = form.querySelector('[name="Ucenik.SifraUcenika"]');
        const ime = form.querySelector('[name="Ucenik.Ime"]');
        const prezime = form.querySelector('[name="Ucenik.Prezime"]');
        const bodovi = form.querySelector('[name="Ucenik.BrojBodova"]');
        const takmicenje = form.querySelector('[name="Ucenik.IDTakmicenja"]');

        sifra?.addEventListener("blur", () => validateSifra(sifra));
        ime?.addEventListener("blur", () => validateRequired(ime, "Ime učenika je obavezno"));
        prezime?.addEventListener("blur", () => validateRequired(prezime, "Prezime učenika je obavezno"));
        bodovi?.addEventListener("blur", () => validateBodovi(bodovi));
        takmicenje?.addEventListener("change", () => validateTakmicenje(takmicenje));

        form.addEventListener("submit", async function (e)
        {
            e.preventDefault();

            const okSifra = await validateSifra(sifra);
            const okIme = validateRequired(ime, "Ime učenika je obavezno");
            const okPrezime = validateRequired(prezime, "Prezime učenika je obavezno");
            const okBodovi = validateBodovi(bodovi);
            const okTakmicenje = validateTakmicenje(takmicenje);

            if (okSifra && okIme && okPrezime && okBodovi && okTakmicenje) {
                form.submit();
            }
        });
    });
})();