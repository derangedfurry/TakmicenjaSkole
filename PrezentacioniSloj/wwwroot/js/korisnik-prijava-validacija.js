(function ()
{
    function prikaziGresku(unos, poruka)
    {
        unos.classList.add("neispravno");
        let greska = unos.parentElement.querySelector(".js-greska-validacije");
        if (!greska) {
            greska = document.createElement("span");
            greska.className = "text-danger js-greska-validacije d-block";
            unos.parentElement.appendChild(greska);
        }
        greska.textContent = poruka;
    }

    function obrisiGresku(unos)
    {
        unos.classList.remove("neispravno");
        const greska = unos.parentElement.querySelector(".js-greska-validacije");
        if (greska) greska.textContent = "";
    }

    function validirajObavezno(unos, poruka)
    {
        if (!(unos.value || "").trim()) {
            prikaziGresku(unos, poruka);
            return false;
        }
        obrisiGresku(unos);
        return true;
    }

    // Email samo ako vrednost sadrži @
    function validirajEmailIliKorisnickoIme(unos)
    {
        const vrednost = (unos.value || "").trim();

        if (!vrednost) {
            prikaziGresku(unos, "Polje nije popunjeno");
            return false;
        }

        if (vrednost.includes("@")) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(vrednost)) {
                prikaziGresku(unos, "Unesite ispravan format email adrese");
                return false;
            }
        }

        obrisiGresku(unos);
        return true;
    }

    document.addEventListener("DOMContentLoaded", function ()
    {
        const forma = document.getElementById("prijavaForma");
        if (!forma) return;

        const emailIliIme = forma.querySelector('[name="EmailIliKorisnickoIme"]');
        const lozinka = forma.querySelector('[name="Lozinka"]');

        emailIliIme?.addEventListener("blur", () =>
            validirajEmailIliKorisnickoIme(emailIliIme));

        lozinka?.addEventListener("blur", () =>
            validirajObavezno(lozinka, "Polje lozinke nije popunjeno"));

        forma.addEventListener("submit", function (e)
        {
            const ispravnoEmail = validirajEmailIliKorisnickoIme(emailIliIme);
            const ispravnoLozinka = validirajObavezno(lozinka, "Polje lozinke nije popunjeno");

            if (!ispravnoEmail || !ispravnoLozinka) {
                e.preventDefault();
                e.stopPropagation();
            }
        });
    });
})();