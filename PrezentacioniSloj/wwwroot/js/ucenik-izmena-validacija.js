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

    async function sifraPostoji(sifra, iskljuciId)
    {
        let url = "/Ucenik/ProveriSifru?sifra=" + encodeURIComponent(sifra);
        if (iskljuciId) {
            url += "&excludeId=" + encodeURIComponent(iskljuciId);
        }
        const odgovor = await fetch(url);
        if (!odgovor.ok) return false;
        const podaci = await odgovor.json();
        return podaci.exists === true;
    }

    function validirajFormatSifre(unos)
    {
        const vrednost = (unos.value || "").trim();

        if (!vrednost) {
            prikaziGresku(unos, "Šifra učenika je obavezna");
            return false;
        }
        if (vrednost.length !== 5) {
            prikaziGresku(unos, "Šifra učenika mora imati tačno 5 karaktera.");
            return false;
        }

        obrisiGresku(unos);
        return true;
    }

    async function validirajSifru(unos, iskljuciId)
    {
        if (!validirajFormatSifre(unos)) return false;

        try {
            const postoji = await sifraPostoji(unos.value.trim(), iskljuciId);
            if (postoji) {
                prikaziGresku(unos, "Učenik sa ovom šifrom već postoji");
                return false;
            }
        } catch {
            prikaziGresku(unos, "Greška pri proveri šifre");
            return false;
        }

        obrisiGresku(unos);
        return true;
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

    function validirajBodove(unos)
    {
        if (unos.value === "") {
            prikaziGresku(unos, "Broj bodova je obavezan");
            return false;
        }
        const broj = Number(unos.value);
        if (isNaN(broj) || broj < 0 || broj > 100) {
            prikaziGresku(unos, "Broj bodova mora biti između 0 i 100.");
            return false;
        }
        obrisiGresku(unos);
        return true;
    }

    function validirajTakmicenje(unos)
    {
        if (!unos.value || unos.value === "0") {
            prikaziGresku(unos, "Takmičenje je obavezno");
            return false;
        }
        obrisiGresku(unos);
        return true;
    }

    document.addEventListener("DOMContentLoaded", function ()
    {
        const forma = document.getElementById("ucenikIzmenaForma");
        if (!forma) return;

        const idUnos = forma.querySelector("#ucenikIdSakriven") || forma.querySelector('[name="ID"]');
        const sifra = forma.querySelector('[name="SifraUcenika"]');
        const ime = forma.querySelector('[name="Ime"]');
        const prezime = forma.querySelector('[name="Prezime"]');
        const bodovi = forma.querySelector('[name="BrojBodova"]');
        const takmicenje = forma.querySelector('[name="IDTakmicenja"]');
        const iskljuciId = idUnos ? idUnos.value : null;

        sifra?.addEventListener("blur", () => validirajSifru(sifra, iskljuciId));
        ime?.addEventListener("blur", () => validirajObavezno(ime, "Ime učenika je obavezno"));
        prezime?.addEventListener("blur", () => validirajObavezno(prezime, "Prezime učenika je obavezno"));
        bodovi?.addEventListener("blur", () => validirajBodove(bodovi));
        takmicenje?.addEventListener("change", () => validirajTakmicenje(takmicenje));

        forma.addEventListener("submit", async function (e)
        {
            e.preventDefault();

            const sveIspravno =
                (await validirajSifru(sifra, iskljuciId)) &&
                validirajObavezno(ime, "Ime učenika je obavezno") &&
                validirajObavezno(prezime, "Prezime učenika je obavezno") &&
                validirajBodove(bodovi) &&
                validirajTakmicenje(takmicenje);

            if (sveIspravno) {
                forma.submit();
            }
        });
    });
})();