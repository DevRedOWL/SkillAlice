﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillAlice.Models
{
    class GetModel
    {
        Meta meta;

        double version = 0;

        class Meta
        {
            string locale = "";
            string timezone = "";
            string client_id = "";
            Interfaces interfaces;

            class Interfaces
            {
                //"screen": ;
            } 
                             
        }

        class Request
        {

        }

        class Session
        {

        }
    }

/*
    {
      "meta": {
        "locale": "ru-RU",
        "timezone": "Europe/Moscow",
        "client_id": "ru.yandex.searchplugin/5.80 (Samsung Galaxy; Android 4.4)",
        "interfaces": {
          "screen": { }
        }
      },
      "request": {
        "command": "закажи пиццу на улицу льва толстого 16 на завтра",
        "original_utterance": "закажи пиццу на улицу льва толстого, 16 на завтра",
        "type": "SimpleUtterance",
        "markup": {
          "dangerous_context": true
        },
        "payload": {},
        "nlu": {
          "tokens": [
            "закажи",
            "пиццу",
            "на",
            "льва",
            "толстого",
            "16",
            "на",
            "завтра"
          ],
          "entities": [
            {
              "tokens": {
                "start": 2,
                "end": 6
              },
              "type": "YANDEX.GEO",
              "value": {
                "house_number": "16",
                "street": "льва толстого"
              }
            },
            {
              "tokens": {
                "start": 3,
                "end": 5
              },
              "type": "YANDEX.FIO",
              "value": {
                "first_name": "лев",
                "last_name": "толстой"
              }
            },
            {
              "tokens": {
                "start": 5,
                "end": 6
              },
              "type": "YANDEX.NUMBER",
              "value": 16
            },
            {
              "tokens": {
                "start": 6,
                "end": 8
              },
              "type": "YANDEX.DATETIME",
              "value": {
                "day": 1,
                "day_is_relative": true
              }
            }
          ]
        }
      },
      "session": {
        "new": true,
        "message_id": 4,
        "session_id": "2eac4854-fce721f3-b845abba-20d60",
        "skill_id": "3ad36498-f5rd-4079-a14b-788652932056",
        "user_id": "AC9WC3DF6FCE052E45A4566A48E6B7193774B84814CE49A922E163B8B29881DC"
      },
      "version": "1.0"
    }
    */

    class PostModel
    {

    }
}
