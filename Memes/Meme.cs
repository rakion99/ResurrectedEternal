﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Meme

{
    private static string[] Before = new string[]
    {
        "slippery",
        "naughty",
        "unholy",
        "retarded",
        "mental",
        "fckable",
        "dirrty",
        "slutty",
        "grim",
        "r4p3d",
        "creampied",
        "thr0atfcked",
        "legendary",
        "lil",
        "interracial",
        "gay",
        "meme",
        "like",
        "enligtened",
        "free",
        "divine",
        "psychotic",
        "exaggerated",
        "reliable",
        "ransom",
        "heretic",
        "rad",
        "french",
        "disappointed",
        "hellfire",
        "reckoning",
        "hellbarde",
        "reckless",
        "roaming",
        "frenchfry",
        "handy",
        "handsome",
        "rafted",
        "created",
        "heated",
        "geekd",
        "foreign",
        "alien",
        "rap",
        "rook",
        "horde",
        "slapper",
        "randy",
        "fart",
        "rack",
        "$tacks"

    };

    private static string[] After = new string[]
    {
        "destroyer",
        "cumdumpster",
        "drip",
        "twink",
        "pus3y",
        "alien",
        "retardation",
        "d1ckh0l3",
        "aids",
        "sweet",
        "juicy",
        "cheeks",
        "whootie",
        "twerk",
        "slut",
        "h0e",
        "butth0l3",
        "ass",
        "butt",
        "throat",
        "sore",
        "gangbang",
        "god",
        "george",
        "hennessy",
        "ready",
        "homo",
        "hans",
        "feuerstelle",
        "deutsch",
        "cr4ck",
        "panzerfaust",
        "ho",
        "hakk",
        "ALI",
        "rookie",
        "hoodie",
        "hardy",
        "undertaker",
        "f",
        "focal",
        "homo",
        "hen",
        "chicken",
        "cow",
        "horse",
        "mc",
        "fo"
    };

    private static Random r = new Random();

    public static string GenerateName()
    {
        return Before[r.Next(0, Before.Length - 1)] + " " + After[r.Next(0, After.Length - 1)];
    }
}