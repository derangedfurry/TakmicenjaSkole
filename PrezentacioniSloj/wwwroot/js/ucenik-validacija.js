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

    async function sifraPostoji(sifra)
    {
        const url = "/Ucenik/ProveriSifru?sifra=" + encodeURIComponent(sifra);
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

    async function validirajSifru(unos)
    {
        if (!validirajFormatSifre(unos)) return false;

        try {
            const postoji = await sifraPostoji(unos.value.trim());
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
            prikaziGresku(unos, "Izaberite takmičenje");
            return false;
        }
        obrisiGresku(unos);
        return true;
    }

    document.addEventListener("DOMContentLoaded", function ()
    {
        const forma = document.getElementById("ucenikForma");
        if (!forma) return;

        const sifra = forma.querySelector('[name="Ucenik.SifraUcenika"]');
        const ime = forma.querySelector('[name="Ucenik.Ime"]');
        const prezime = forma.querySelector('[name="Ucenik.Prezime"]');
        const bodovi = forma.querySelector('[name="Ucenik.BrojBodova"]');
        const takmicenje = forma.querySelector('[name="Ucenik.IDTakmicenja"]');

        sifra?.addEventListener("blur", () => validirajSifru(sifra));
        ime?.addEventListener("blur", () => validirajObavezno(ime, "Ime učenika je obavezno"));
        prezime?.addEventListener("blur", () => validirajObavezno(prezime, "Prezime učenika je obavezno"));
        bodovi?.addEventListener("blur", () => validirajBodove(bodovi));
        takmicenje?.addEventListener("change", () => validirajTakmicenje(takmicenje));

        forma.addEventListener("submit", async function (e)
        {
            e.preventDefault();

            const sifraIspravna = await validirajSifru(sifra);
            const imeIspravno = validirajObavezno(ime, "Ime učenika je obavezno");
            const prezimeIspravno = validirajObavezno(prezime, "Prezime učenika je obavezno");
            const bodoviIspravni = validirajBodove(bodovi);
            const takmicenjeIspravno = validirajTakmicenje(takmicenje);

            if (sifraIspravna && imeIspravno && prezimeIspravno && bodoviIspravni && takmicenjeIspravno) {
                forma.submit();
            }
        });
    });
})();