/**
 * Copyright (c) 2017-present, PFW Contributors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License is
 * distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See
 * the License for the specific language governing permissions and limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using PFW.Model.Armory;

namespace PFW
{
	public static class ConfigReader
    {
        public static Unit FindUnit(string unitId)
        {
            MatchCollection matches =
                    Regex.Matches(unitId, @"^(Unit__)([A-Z]+)(?:--)([-_a-zA-Z0-9]+)");
            string categoryKey = matches[0].Groups[2].Value;
            string modelDesignation = matches[0].Groups[3].Value;

            TextAsset configFile = Resources.Load<TextAsset>($"{categoryKey}/{modelDesignation}");
            UnitConfig config = JsonUtility.FromJson<UnitConfig>(configFile.text);

            return new Unit(config);
        }

        public static Deck FindDeck(string deckName)
        {
            TextAsset configFile = Resources.Load<TextAsset>($"Decks/{deckName}");
            DeckConfig config = JsonUtility.FromJson<DeckConfig>(configFile.text);

            return new Deck(config);
        }
    }
}