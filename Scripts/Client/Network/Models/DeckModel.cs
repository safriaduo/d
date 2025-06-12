using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dawnshard.Network
{
    public class DeckModel
    {
        public string Name { get; set; }

        public List<int> CardSetIds { get; set; }

        public int DeckSpriteId { get; set; } = 0;

        public DeckModel() { }

        public DeckModel(DeckDTO deck)
        {
            //TODO: replace
            Name = deck.Name;
            //CardSetIds = deck.CardSetIDs.ToList();
        }

    }
}
