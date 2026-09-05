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

    async function nazivPostoji(naziv, id)
    {
        let url = "/Predmet/ProveriNaziv?naziv=" + encodeURIComponent(naziv);
        if (id) {
            url += "&id=" + encodeURIComponent(id);
        }
        const odgovor = await fetch(url);
        if (!odgovor.ok) return false;
        const podaci = await odgovor.json();
        return podaci.exists === true;
    }

    async function validirajNaziv(unos, id)
    {
        const vrednost = (unos.value || "").trim();

        if (!vrednost) {
            prikaziGresku(unos, "Naziv predmeta je obavezan");
            return false;
        }

        try {
            const postoji = await nazivPostoji(vrednost, id);
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
        const forma = document.getElementById("predmetIzmenaForma");
        if (!forma) return;

        const idUnos = forma.querySelector('[name="ID"]');
        const nazivUnos = forma.querySelector('[name="NazivPredmeta"]');
        const id = idUnos ? idUnos.value : null;

        nazivUnos?.addEventListener("blur", () => validirajNaziv(nazivUnos, id));

        forma.addEventListener("submit", async function (e)
        {
            e.preventDefault();
            const nazivIspravan = await validirajNaziv(nazivUnos, id);
            if (nazivIspravan) {
                forma.submit();
            }
        });
    });
})();