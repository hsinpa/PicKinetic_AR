﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFlag 
{
    public class Event
    {
        public const string GameStart = "event@game_start";

        public const string OnAREnable = "event@ar_enable";
        public const string OnARDisable = "event@ar_disable";

        public const string OnPhotoAlbumOpen = "event@open_photo_album";
        public const string OnPhotoAlbumClose = "event@close_photo_album";


    }
}