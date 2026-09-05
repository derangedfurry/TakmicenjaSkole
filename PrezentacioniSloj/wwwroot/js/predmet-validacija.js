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

    async function nazivPostoji(naziv)
    {
        const url = "/Predmet/ProveriNaziv?naziv=" + encodeURIComponent(naziv);
        const odgovor = await fetch(url);
        if (!odgovor.ok) return false;
        const podaci = await odgovor.json();
        return podaci.exists === true;
    }

    function validirajId(unos)
    {
        const vrednost = (unos.value || "").trim();

        if (!vrednost) {
            prikaziGresku(unos, "Id predmeta je obavezan");
            return false;
        }
        if (vrednost.length !== 5) {
            prikaziGresku(unos, "ID predmeta mora imati tačno 5 karaktera");
            return false;
        }

        obrisiGresku(unos);
        return true;
    }

    async function validirajNaziv(unos)
    {
        const vrednost = (unos.value || "").trim();

        if (!vrednost) {
            prikaziGresku(unos, "Naziv predmeta je obavezan");
            return false;
        }

        try {
            const postoji = await nazivPostoji(vrednost);
            if (postoji) {
                prikaziGresku(unos, "Predmet sa ovim nazivom već postoji");
                return false;
            }
        } catch {
            prikaziGresku(unos, "Greška pri proveri naziva");
            return false;
        }

        obrisiGresku(unos);
        return true;
    }

    document.addEventListener("DOMContentLoaded", function ()
    {
        const forma = document.getElementById("predmetForma");
        if (!forma) return;

        const idUnos = forma.querySelector('[name="ID"]');
        const nazivUnos = forma.querySelector('[name="NazivPredmeta"]');

        idUnos?.addEventListener("blur", () => validirajId(idUnos));
        nazivUnos?.addEventListener("blur", () => validirajNaziv(nazivUnos));

        forma.addEventListener("submit", async function (e)
        {
            e.preventDefault();

            const idIspravan = validirajId(idUnos);
            const nazivIspravan = await validirajNaziv(nazivUnos);

            if (idIspravan && nazivIspravan) {
                forma.submit();
            }
        });
    });
})();