using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CategoryScript : MonoBehaviour
{
    //Variable instantiation
    private string[][] histCat = new string[35][];
    private string[][] langCat = new string[35][];
    private string[][] ricCat = new string[35][];
    private string[][] vgCat = new string[35][]; 

    public string[][] histMixed;
    public string[][] langMixed;
    public string[][] ricMixed;
    public string[][] vgMixed;

    /// <summary>
    /// Call ShuffleLists() on start of execution.
    /// </summary>
    private void Start() {
        ShuffleLists();
    }

    /// <summary>
    /// Call the GetMixedList() function for each category.
    /// </summary>
    public void ShuffleLists() {
        Populate();

        histMixed = GetMixedList(histCat);
        langMixed = GetMixedList(langCat);
        ricMixed = GetMixedList(ricCat);
        vgMixed = GetMixedList(vgCat);
    }

    /// <summary>
    /// Shuffle questions in the array.
    /// </summary>
    /// <param name="l">ordered array</param>
    /// <returns>shuffled array</returns>
    public string[][] GetMixedList(string[][] l) {

        string[][] array2D = l;

        for (int i = 0; i < array2D.Length; i++) {
            int rgn = Random.Range(0, array2D.Length - 1);
            string[] temp = array2D[i];
            array2D[i] = array2D[rgn];
            array2D[rgn] = temp;
        }

        return array2D;
    }

    /// <summary>
    /// Populate arrays.
    /// </summary>
    private void Populate() {

        //Populate History Category
        histCat[0]  = new string[] { "What was the name of the company Steve Jobs started when he left Apple in 1985?", "NeXT", "Microsoft", "Pear", "Android" };
        histCat[1]  = new string[] { "What was the first computer mouse made out of and how many buttons did it have?", "Wood, 1", "Plastic, 2", "Metal, 1", "Glass, 2" };
        histCat[2]  = new string[] { "Who launched the first website?", "Cern", "Google", "MIT", "IBM" };
        histCat[3]  = new string[] { "The versions of the macOS operating system are named after what?", "Both places and big cats", "Places in California", "Big Cats", "Neither" };
        histCat[4]  = new string[] { "Early in development, Windows was known by what name?", "Interface Manager", "Gates Interface", "Graphical Assistant", "Compu-Manage" };
        histCat[5]  = new string[] { "When did Charles Babbage create the first mechanical computer?", "1822", "1922", "1856", "1945" };
        histCat[6]  = new string[] { "What was the first Android mobile phone?", "HTC Dream", "T-Mobile Sidekick", "Blackberry", "iPhone 3" };
        histCat[7]  = new string[] { "How much storage space did the first flash drive available to consumers contain?", "8MB", "16MB", "4GB", "8GB" };
        histCat[8]  = new string[] { "When was the first text message sent?", "1992", "1998", "1988", "2001" };
        histCat[9]  = new string[] { "What was the first product created by Sony?", "Electric Rice Cooker", "TV", "VCR", "PlayStation" };
        histCat[10] = new string[] { "What operating system was developed by Google?", "Android", "Windows", "iOS", "Linux" };
        histCat[11] = new string[] { "What was the first programmable general-purpose electronic digital computer?", "ENIAC", "BINAC", "C3PO", "VRXS" };
        histCat[12] = new string[] { "What was the first personal computer?", "Altair", "Apple I", "Apple Lisa", "IBM 610" };
        histCat[13] = new string[] { "What year was Google founded?", "1998", "1985", "2000", "1992" };
        histCat[14] = new string[] { "Who is known as the first computer programmer?", "Ada Lovelace", "Bill Gates", "Alan Turing", "Steve Jobs" };
        histCat[15] = new string[] { "What was the first full length animated movie to be completely computer generated?", "Toy Story", "A Bug's Life", "Beauty and the Beast", "James and the Giant Peach" };
        histCat[16] = new string[] { "Which animation studio produced the first completely computer animated film?", "Pixar", "Walt Disney Animation", "DreamWorks", "Illumination" };
        histCat[17] = new string[] { "What year was the first iPhone released?", "2007", "2005", "2010", "2002" };
        histCat[18] = new string[] { "What is the maximum number of characters a Tweet can contain?", "280", "140", "70", "560" };
        histCat[19] = new string[] { "Which is not a Microsoft product?", "Keynote", "Excel", "Powerpoint", "Word" };
        histCat[20] = new string[] { "Intel introduced the 64-bit chip in what year?", "1993", "1975", "1986", "1999" };
        histCat[21] = new string[] { "When was Bitcoin invented?", "2008", "2018", "2012", "2020" };
        histCat[22] = new string[] { "What was the first computer virus called?", "Creeper", "Ghost", "Monster", "Corona" };
        histCat[23] = new string[] { "What does IBM stand for?", "International Business Machines", "Incorporated Binary Mathematics", "Industrial Business Marketing", "Incredible Business Machine" };
        histCat[24] = new string[] { "Approximately how many songs could the first iPod store?", "1000", "500", "5000", "100" };
        histCat[25] = new string[] { "What were Android's OS's previously names after?", "Desserts", "Deserts", "Wildlife", "Botanical life" };
        histCat[26] = new string[] { "Who is the lesser known Stephen who co-founded Apple alongside Steve Jobs?", "Steve Wozniak", "Steven Spielberg", "Stephen King", "Steve Rogers" };
        histCat[27] = new string[] { "What kernel is Android OS derived from?", "Linux", "Windows", "Unix", "Apple" };
        histCat[28] = new string[] { "What does PDA stand for?", "Personal Digital Assistant", "Personal Digital Associate", "Privte Data Access", "Pediatric Doctors Anonymous" };
        histCat[29] = new string[] { "What does GNU stand for?", "GNU's Not Unix", "It's not an acronym", "Going Nuts United", "Graphical Networking Unit" };
        histCat[30] = new string[] { "Who created Unix?", "Dennis Ritchie", "Steve Wozniak", "Richard Stallman", "Alan Turing" };
        histCat[31] = new string[] { "Who was one of the founders of Microsoft?", "Paul Allen", "Paul Graham", "James Nell", "David Reed" };
        histCat[32] = new string[] { "Who was the original voice of Siri?", "Susan Bennett", "Susan Caplin", "Laura Bailey", "Jane Anderson" };
        histCat[33] = new string[] { "Which of these is NOT a real virtual assistant?", "Emma", "Siri", "Cortana", "Alexa" };
        histCat[34] = new string[] { "Which of these is NOT a real search engine?", "Findit", "DuckDuckGo", "Dogpile", "HotBot" };

        //Populate Languages Category
        langCat[0]  = new string[] { "What is the most commonly used programming language?", "Java", "Python", "C#", "R" };
        langCat[1]  = new string[] { "Which is not a primitive data type in Java?", "String", "Int", "char", "Boolean" };
        langCat[2]  = new string[] { "What is 15 in binary?", "1111", "1001", "1101", "0101" };
        langCat[3]  = new string[] { "What is the range of values that can be stored in a variable that is of type byte?", "0 to 225", "-128 to 127", "-225 to 225", "0 to 4292967295" };
        langCat[4]  = new string[] { "What is the value of A when the following is executed? A = 12; A = A % 10;", "2", "1.2", "1", "0" };
        langCat[5]  = new string[] { "Which is not a real programming language?", "C!", "C", "C#", "C++" };
        langCat[6]  = new string[] { "If value = 20, what is the output of the command print(value)?", "20", "value", "value = 20", "value 20" };
        langCat[7]  = new string[] { "Which represents -15 as a 6-bit signed binary number?", "101111", "001111", "101100", "111101" };
        langCat[8]  = new string[] { "A loop that never ends is considered what type of loop?", "Infinite", "Forever", "Recursive", "Crazy" };
        langCat[9]  = new string[] { "All computers execute what?", "Machine language program", "Java program", "Easy program", "Basic program" };
        langCat[10] = new string[] { "What is the standard notation for comments in C#", "//", "##", "\\\\", "!!" };
        langCat[11] = new string[] { "Which of the following keywords is used to refer to a member of a base class from a subclass in Java?", "super", "master", "greater", "upper" };
        langCat[12] = new string[] { "Which keyword is used to inherit a class in Java?", "extends", "inherits", "gets", "has" };
        langCat[13] = new string[] { "Which language is commonly used for web design?", "HTML", "CPP", "TMDZ", "KLRB" };
        langCat[14] = new string[] { "A class that cannot be instantiated is known as what?", "Abstract", "Pretend", "Imitation", "Theoretical" };
        langCat[15] = new string[] { "Which keyword is used by a method to refer to the object that invoked it?", "this", "that", "object", "in" };
        langCat[16] = new string[] { "What is the answer to the expression 22 % 3?", "1", "7", "0", "8" };
        langCat[17] = new string[] { "In Java, the Scanner class is included in which package?", "Java.io", "Java.lang", "Java.util", "Java.math" };
        langCat[18] = new string[] { "What is the index of the first element of an array?", "0", "1", "-1", "2" };
        langCat[19] = new string[] { "What is the process of removing an element from a stack called?", "pop", "push", "peak", "pull" };
        langCat[20] = new string[] { "What is the decimal value of the hexadecimal number 19B?", "411", "19", "103", "8" };
        langCat[21] = new string[] { "In Java, how many Bytes is an integer?", "4", "2", "8", "1" };
        langCat[22] = new string[] { "What does JVM stand for?", "Java Virtual Machine", "Java Vital Memory", "Java Volatile Memory", "Java Video Machine" };
        langCat[23] = new string[] { "What is the binary value of 401?", "110010001", "010111001", "001101110", "1111000000001" };
        langCat[24] = new string[] { "Which is not a correct primitive data type in C++ programming?", "real", "float", "int", "double" };
        langCat[25] = new string[] { "Which of these statements is True?", "(True OR False) AND True", "(True AND False) OR False", "(True AND True) AND False", "(Not True AND False) OR False" };
        langCat[26] = new string[] { "How many bits are in 64 bytes?", "512", "1024", "64", "32" };
        langCat[27] = new string[] { "Which of these statements is False?", "Not True OR False", "Not False AND True", "True OR True", "Not False OR True" };
        langCat[28] = new string[] { "Which language is not object-oriented?", "C", "C++", "Java", "Perl" };
        langCat[29] = new string[] { "Which is not a data structure?", "Collection", "Stack", "Vector", "List" };
        langCat[30] = new string[] { "Which one of the following is the AND operator in Java?", "&&", "||", "<>", "AND" };
        langCat[31] = new string[] { "Which international coding system represents all the characters of all the world's alphabet?", "ASCII", "Unicode", "Java", "EBCDIC" };
        langCat[32] = new string[] { "Which of these loop statements will execute at least once in Java?", "do()", "for()", "while()", "if()" };
        langCat[33] = new string[] { "What is the running-time efficiency of merge sort in its worst case?", "O(n^2)", "O(n)", "O(logn)", "35" };
        langCat[34] = new string[] { "In Java, if days is a two-dimensional array, how would the row ‘r’ and column ‘c’ be assigned?", "days[r][c]", "days[c][r]", "days[c].length[r]", "days[r,c]" };

        //Populate RIC Category
        ricCat[0]  = new string[] { "What is the course code for Software Engineering?", "CSCI 401", "CS 401", "CSCI 499", "CS 499" };
        ricCat[1]  = new string[] { "What is the correct spelling of Seth's middle name?", "Jarryd", "Jarod", "Jared", "Gerad" };
        ricCat[2]  = new string[] { "What is the correct spelling of Taline's last name?", "Mkrtschjan", "Makertschajan", "Makirtajan", "Mkrtaschjen" };
        ricCat[3]  = new string[] { "What is the correct pronunciation of Dylan's last name?", "Grahnd Jahn", "Grund Jean", "Green Jean", "Grand Jane" };
        ricCat[4]  = new string[] { "What is the correct spelling of Matt's last name?", "SanSouci", "Sansouci", "Sansouzi", "SansouZi" };
        ricCat[5]  = new string[] { "Which country is Dr. Elfouly from?", "Egypt", "Lebanon", "Morocco", "Syria" };
        ricCat[6]  = new string[] { "Who is allowed to park in parking lot K at RIC?", "Anyone", "Students only", "Faculty only", "Residents only" };
        ricCat[7]  = new string[] { "What is RIC's address?", "600 Mt. Pleasant Ave", "450 College Road", "1000 Fruit Hill Ave", "900 Smith St." };
        ricCat[8]  = new string[] { "Who is the current chair of the computer science department?", "Dr. Bain", "Dr. Sarawagi", "Dr. Costa", "Dr. Abrahamson" };
        ricCat[9]  = new string[] { "Which of these courses is not required for the computer science major?", "CSCI 101", "CSCI 212", "CSCI 312", "CSCI 435" };
        ricCat[10] = new string[] { "What building is the computer science department based out of?", "Craig Lee", "Horace Mann", "Gaige", "Clarke" };
        ricCat[11] = new string[] { "Assuming no special permission is required, what is the highest number of credits one can possible complete in on year at RIC?", "52", "36", "44", "60" };
        ricCat[12] = new string[] { "What is the threshold GPA to graduate summa cum laude at RIC?", "3.85", "4.0", "3.95", "3.75" };
        ricCat[13] = new string[] { "Who teaches CSCI 415?", "Marc René", "Mark Rene", "Marc Rênee", "Mark Renée" };
        ricCat[14] = new string[] { "What course is CSCI 309?", "Object Oriented Design", "Data Structures", "Organization of Programming Languages", "Functional Programming" };
        ricCat[15] = new string[] { "What is the correct extension for student email addresses at RIC?", "email.ric.edu", "ric.edu", "email.edu", "ric.email.edu" };
        ricCat[16] = new string[] { "Before joining with the computer information systems department, which department was linked with the computer science department?", "Mathematics", "The Department was not linked to another", "Physics", "Physical Science" };
        ricCat[17] = new string[] { "Which of these courses also satisfies a general education requirement?", "CSCI 423", "CSCI 309", "CSCI 401", "CSCI 435" };
        ricCat[18] = new string[] { "Who is allowed to park in parking lot J", "Students", "Anyone", "Residents", "Faculty" };
        ricCat[19] = new string[] { "Which of these buildings does NOT exist on RIC campus?", "White Cabin", "Greenhouse", "Yellow Cottage", "Murray Center" };
        ricCat[20] = new string[] { "Which university did Dr. Sarawagi receive her PhD from?", "North Eastern University", "Massachusetts Institute of Technology", "Harvard", "Rhode Island College" };
        ricCat[21] = new string[] { "What course is CSCI 314?", "This course does not exist", "Computer Organization and Architrecture III", "Organization of Operating Systems", "Analysis of Algorithms" };
        ricCat[22] = new string[] { "What is the minimum number of credits necessary to graduate with a BA in Computer Science?", "120", "49", "51", "86" };
        ricCat[23] = new string[] { "Where does RIC hold its graduation ceremony?", "The Dunkin' Donuts Center", "On Campus", "The Omni Hotel in Providence", "The Providence Performing Art Center" };
        ricCat[24] = new string[] { "What is the name of the library on RIC's campus?", "Adams", "Washington", "Jefferson", "Lincoln" };
        ricCat[25] = new string[] { "What year was RIC founded?", "1854", "1923", "1888", "1790" };
        ricCat[26] = new string[] { "How big is RIC's campus?", "180 acres", "90 acres", "300 acres", "220 acres" };
        ricCat[27] = new string[] { "What are RIC's colors?", "Burgundy, Gold and White", "Maroon, Yellow and White", "Red, Gold and White", "Blood, Gold and Snow" };
        ricCat[28] = new string[] { "What is the minimum GPA required for graduation?", "2.0", "2.5", "3.0", "4.0" };
        ricCat[29] = new string[] { "In what year did RIC relocate to its current location in the Mount Pleasant Area of Providence?", "1958", "1924", "1995", "1976" };
        ricCat[30] = new string[] { "What was Rhode Island College's former name?", "All of these", "Rhode Island State Normal School", "Rhode Island Normal School", "Rhode Island College of Education" };
        ricCat[31] = new string[] { "What is the name of RIC's newspaper?", "The Anchor", "The RIC Times", "The Rhode Island Gazette", "The Daily Bugle" };
        ricCat[32] = new string[] { "Who is the current president of RIC", "Frank Sánchez", "Frank Sinatra", "Frank Oz", "Frank Ocean" };
        ricCat[33] = new string[] { "Which actress is an alumna of RIC", "Viola Davis", "Emma Watson", "Meryl Streep", "Scarlett Johansson" };
        ricCat[34] = new string[] { "Which of the following is the RIC radio station?", "90.7 WXIN", "92.3 WPRO", "95.5 WBRU", "94.1 WHJY" };

        //Populate Video Game Category
        vgCat[0]  = new string[] { "Which of these was released earliest?", "Pong", "Donkey Kong", "Pac-Man", "Space Invaders" };
        vgCat[1]  = new string[] { "What intellectual property was Donkey Kong meant to be before being retooled?", "Popeye", "Flinstones", "Scooby-Doo", "Looney Tunes" };
        vgCat[2]  = new string[] { "Which of these wasn't originally a Mario game?", "Super Mario Bros 2", "Super Mario 3", "Super Mario World", "Super Mario Sunshine" };
        vgCat[3]  = new string[] { "How was Ganondorf's name spelled in the original Legend of Zelda?", "Gannon", "Ganon", "Gannondorf", "GanonDorf" };
        vgCat[4]  = new string[] { "What was the first released home video game console?", "Magnavox Odyssey", "Nintendo Entertainment System", "Famicom", "Color TV Game" };
        vgCat[5]  = new string[] { "What was the first released portable gaming console?", "Game & Watch", "Gameboy", "Nintendo DS", "Playstation Portable" };
        vgCat[6]  = new string[] { "Which of the following characters is NOT playable in the original Super Smash Brothers?", "Bowser", "Ness", "Captain Falcon", "Luigi" };
        vgCat[7]  = new string[] { "What is the name of the BLUE ghost in Pac-Man?", "Inky", "Blinky", "Pinky", "Clyde" };
        vgCat[8]  = new string[] { "What is the name of the ORANGE ghost in Ms. Pac-Man?", "Sue", "Clyde", "Tim", "Walter" };
        vgCat[9]  = new string[] { "What TV serie's theme is riffed as part of Donkey Kong's soundtrack?", "Dragnet", "Hawaii 5-0", "Twilight Zone", "MacGyver" };
        vgCat[10] = new string[] { "What is the highest possible score in the original Pac-Man?", "3,333,360", "9,999,999", "4,294,967,296", "65,535" };
        vgCat[11] = new string[] { "What happens on level 256 of Pac-Man as a result of overflow?", "The right half of the screen is garbled", "The entire screen is garbled", "The left half of the screen is garbled", "It resets to level 1" };
        vgCat[12] = new string[] { "What was the final home console released by Sega?", "Dreamcast", "Genesis", "Saturn", "Jaguar" };
        vgCat[13] = new string[] { "Who is Sonic's sidekick?", "Tails", "Shadow", "Knuckles", "Amy" };
        vgCat[14] = new string[] { "Which of these series is not produced by Capcom?", "Final Fantasy", "Mega Man", "Devil May Cry", "Ace Attorney" };
        vgCat[15] = new string[] { "Which of these games features the Abrahamic God as the final boss?", "Shin Megami Tensei II", "Devil May Cry", "Dante's Inferno", "Darksiders" };
        vgCat[16] = new string[] { "Who famously dies during Final Fantasy VII?", "Aerith", "Barrett", "Josef", "Tellah" };
        vgCat[17] = new string[] { "Who developed the CD add-on for Nintendo's CD-i?", "Phillips", "Sony", "Microsoft", "Apple" };
        vgCat[18] = new string[] { "Which of these is considered to have established Japanese console RPGs?", "Dragon Quest", "Final Fantasy", "Secret of Mana", "Chrono Trigger" };
        vgCat[19] = new string[] { "What video game franchise has the art done by the same artist as the Dragonball franchise?", "Dragon Quest", "Final Fantasy", "The Legend of Zelda", "Fire Emblem" };
        vgCat[20] = new string[] { "Which video game samples Bach's Toccata and Fugue in D minor?", "Donkey Kong Jr.", "Castlevania", "Sonic the Hedgehog", "Resident Evil" };
        vgCat[21] = new string[] { "Who composed the music for Super Mario Bros?", "Koji Kondo", "Nobuo Uematsu", "Koichi Sugiyama", "Yoko Shimomura" };
        vgCat[22] = new string[] { "Which of these games came with a Robotic Operating Buddy (R.O.B.) to help sales in North America?", "Gyromite", "Mega Man", "Phantasy Star", "The Legend of Zelda" };
        vgCat[23] = new string[] { "Which of these franchises features the infamous “Mecha-Hitler”?", "Wolfenstein", "Devil May Cry", "Doom", "Resident Evil" };
        vgCat[24] = new string[] { "In order to bypass censors how was Hitler edited for the English release of Persona 2?", "All of these are correct", "He was given sunglasses", "All swastika symbols were replaced with plus symbols", "He had his name changed to “Fuhrer”" };
        vgCat[25] = new string[] { "How much memory space did the PlayStation 1 memory card contain?", "128 kilobytes", "8 megabytes", "4 gigabytes", "64 kilobytes" };
        vgCat[26] = new string[] { "What is the best-selling video game console of all time?", "PlayStation 2", "Nintendo Wii", "Xbox 360", "Sega Genesis" };
        vgCat[27] = new string[] { "What year was Nintendo founded", "1889", "1989", "1955", "1964" };
        vgCat[28] = new string[] { "Who is the main villain in the Crash Bandicoot franchise?", "Dr. Neo Cortex", "Dingodile", "Dr. Nitrus Brio", "Tiny" };
        vgCat[29] = new string[] { "How many playable characters were there in the first Super Mario Smash Bros. game?", "12", "24", "2", "16" };
        vgCat[30] = new string[] { "What year was the original Tomb Raider released?", "1996", "1995", "2000", "1992" };
        vgCat[31] = new string[] { "Which game came bundled with the Nintendo Wii?", "Wii Sports", "Mario Kart Wii", "Super Mario Bros. Wii", "Wii Fit" };
        vgCat[32] = new string[] { "What is the best-selling video game of all time?", "Minecraft", "Tetris", "Wii Sports", "Super Mario Bros." };
        vgCat[33] = new string[] { "What is the best-selling handheld console to date?", "Nintendo DS", "PlayStation Portable", "Game Boy Color", "Game Boy Advance" };
        vgCat[34] = new string[] { "Who released the first flight simulator game?", "Microsoft", "Nintendo", "Sony", "Atari" };
    }
}
