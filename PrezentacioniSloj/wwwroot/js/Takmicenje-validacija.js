(function ()
{
    function uDateTimeLocalVrednost(datum)
    {
        const dopuni = (n) => n.toString().padStart(2, "0");
        return datum.getFullYear() + "-" +
            dopuni(datum.getMonth() + 1) + "-" +
            dopuni(datum.getDate()) + "T" +
            dopuni(datum.getHours()) + ":" +
            dopuni(datum.getMinutes());
    }

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

    function validirajFormu(forma)
    {
        let ispravno = true;

        // Obavezna polja (master – takmičenje)
        const obavezniSelektor = [
            '[name="Takmicenje.NazivTakmicenja"]',
            '[name="Takmicenje.NazivPredmetaTakmicenja"]',
            '[name="Takmicenje.LokacijaTakmicenja"]',
            '[name="Takmicenje.TipTakmicenja"]',
            '[name="Takmicenje.DatumTakmicenja"]'
        ];

        obavezniSelektor.forEach(sel =>
        {
            const element = forma.querySelector(sel);
            if (!element) return;

            if (!element.value || !element.value.trim()) {
                prikaziGresku(element, "Ovo polje je obavezno.");
                ispravno = false;
            } else {
                obrisiGresku(element);
            }
        });

        // Datum ne sme biti u budućnosti
        const datumUnos = forma.querySelector('[name="Takmicenje.DatumTakmicenja"]');
        if (datumUnos && datumUnos.value) {
            const izabrani = new Date(datumUnos.value);
            const sada = new Date();
            if (izabrani > sada) {
                prikaziGresku(datumUnos, "Datum takmičenja ne može biti u budućnosti.");
                ispravno = false;
            }
        }

        // Redovi učenika
        const redovi = forma.querySelectorAll("#uceniciBody tr");
        if (redovi.length === 0) {
            alert("Dodajte bar jednog učenika.");
            ispravno = false;
        }

        redovi.forEach(red =>
        {
            const sifra = red.querySelector('input[name*="SifraUcenika"]');
            const ime = red.querySelector('input[name*=".Ime"]');
            const prezime = red.querySelector('input[name*="Prezime"]');
            const bodovi = red.querySelector('input[name*="BrojBodova"]');

            if (sifra) {
                if (!sifra.value || sifra.value.length !== 5) {
                    prikaziGresku(sifra, "Šifra mora imati tačno 5 karaktera.");
                    ispravno = false;
                } else {
                    obrisiGresku(sifra);
                }
            }

            if (ime && !ime.value.trim()) {
                prikaziGresku(ime, "Ime je obavezno.");
                ispravno = false;
            } else if (ime) {
                obrisiGresku(ime);
            }

            if (prezime && !prezime.value.trim()) {
                prikaziGresku(prezime, "Prezime je obavezno.");
                ispravno = false;
            } else if (prezime) {
                obrisiGresku(prezime);
            }

            if (bodovi) {
                const broj = Number(bodovi.value);
                if (bodovi.value === "" || isNaN(broj) || broj < 0 || broj > 100) {
                    prikaziGresku(bodovi, "Broj bodova mora biti između 0 i 100.");
                    ispravno = false;
                } else {
                    obrisiGresku(bodovi);
                }
            }
        });

        return ispravno;
    }

    document.addEventListener("DOMContentLoaded", function ()
    {
        const forma = document.getElementById("takmicenjeForma");
        if (!forma) return;

        // Maksimalni datetime-local = sada
        const datumUnos = forma.querySelector('[name="Takmicenje.DatumTakmicenja"]');
        if (datumUnos) {
            datumUnos.max = uDateTimeLocalVrednost(new Date());
        }

        forma.addEventListener("submit", function (e)
        {
            if (!validirajFormu(forma)) {
                e.preventDefault();
                e.stopPropagation();
            }
        });
    });
})();