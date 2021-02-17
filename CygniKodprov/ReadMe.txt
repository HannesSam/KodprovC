Mashup API
Av: Hannes Samskog

Att köra API:t
url: https://localhost:44310/Mashup/ArtistOrBand
exempel förfrågan: https://localhost:44310/Mashup/ArtistOrBand?MBID=c2764f38-febf-4e47-a0d7-687980aabf38

Designstruktur
När jag gjorde strukturen så behövde jag tänka mig möjliga sätt att expandera appen för att kunna göra en lätt struktur att följa. Så min tanke för att expandera vidare på denna app innebär att det skulle vara en
mashup app för flera typer av mediaformat. Tex skulle man kunna lägga till en endpoint som hette "Movie" som returnerade en film från imdb:s databas och sedan hämtade ratings och liknande från en rad andra databaser
och gjorde en sammanslagning av all data. Så tanken var att appen är designad för att lätt kunna lägga till nya funktioner som innebär relativt små och självständiga funktioner som inte delar så mycket kod
mellan varandra då de anropar olika typer av externa tjänster. 

Kvar att göra
Jag gjorde lite avgränsningar till mig själv för att inte spendera för mycket tid och jag redogör för de här:
-Implementera logging.
-Läsa på mer om de externa API:s som använder för att kunna logga och kasta mer talande fel beroende på vilken http kod som returneras. 
-Dokumentera applikationen för användare i Swagger eller liknande.
-Undersöka prestandan mer och implementera lösningar särskilt när det kommer till alla requests vi gör för att skapa albumen. Nu väntar exekveringen på varje request innan den går vidare vilket 
inte är optimalt.

Sedan har jag såklart mer att säga kring allt detta och ser fram emot att förhoppningsvis få höra era åsikter om allt på en framtida teknikintervju!