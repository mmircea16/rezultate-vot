import React from "react";

export const AboutProjectPage = () => {
  return (
    <div
      className="static-container"
    >
        <h2>DESPRE PROIECT</h2>

        <p>Momentul <span className="emphasize">2016</span> a fost unul de cotitură în istoria alegerilor din țara noastră, fiind anul în care în <span className="emphasize">Monitorizare Vot a digitalizat întreg procesul de observare</span>, transformând practic tot fenomenul de observare din ceva post-factum în ceva instant.</p>

        <p>2019.</p>

        <p className="emphasize">Rezultate Vot, transparentizează întreg procesul electoral furnizând în timp real, într-o formă grafică intuitivă, ușor de înțeles, datele oficiale furnizate de către AEP și Birourile Electorale cât și parte din datele preluate prin aplicația Monitorizare Vot dezvoltată de Code for Romania, despre alegerile din România.</p>

        <p>Informații furnizate:</p>

        <ul>
            <li>rezultatele parțiale ale alegerilor, imediat după închiderea urnelor de votare;</li>
            <li>prezență și vot la nivel de județ;</li>
            <li>rezultatele observării independente a alegerilor</li>
            <li>istoricul electoral al României de-a lungul timpului - într-o etapă de dezvoltare ulterioară.</li>
        </ul>

        <p>În anul 2020 platforma va agrega și mai multe informații utile. Voluntarii Code for Romania lucrează în continuare pentru a adăuga module care vor permite ca datele istorice despre votul din România să poată fi vizualizate și înțelese astfel mai bine.</p>

        <p>Mulțumim tuturor celor care au făcut acest lucru posibil într-un timp record!</p>

        <p className="mt-5">Proiect realizat în totalitate prin implicarea voluntarilor Code for Romania:</p>

        <p className="mt-3 text-center">Bogdan Bujdea</p>

        <p className="mt-3 text-center">Adelina Nicolov</p>

    </div>
  );
}
