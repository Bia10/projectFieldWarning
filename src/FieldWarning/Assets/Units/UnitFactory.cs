﻿/**
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
using UnityEngine;

using PFW.Model.Game;
using PFW.ECS;
using Unity.Transforms;
using Unity.Entities;

namespace PFW.Ingame.Prototype
{
    public class UnitFactory
    {
        private MatchSession _session { get; }

        public UnitFactory(MatchSession session)
        {
            _session = session;
        }

        public GameObject FindPrefab(UnitType type)
        {
            GameObject unit;

            switch (type) {
            case UnitType.Tank:
                unit = Resources.Load<GameObject>("Tank");
                //label.GetComponentInChildren<Text>().text = "M1A2 Abrams";
                break;
            case UnitType.AFV:
                unit = Resources.Load<GameObject>("AFV");
                break;
            case UnitType.Infantry:
                var obj = new GameObject();
                var b = obj.AddComponent<InfantryBehaviour>();
                b.enabled = false;
                unit = obj;
                break;
            case UnitType.Arty:
                unit = Resources.Load<GameObject>("Arty");
                break;
            default:
                unit = null;
                break;
            }

            //unit.GetComponent<UnitLabelAttacher>().Label = label;

            return unit;
        }

        public GameObject MakeUnit(GameObject prefab, Color minimapColor)
        {
            GameObject unit = Object.Instantiate(prefab);
            AddMinimapIcon(unit, minimapColor);
            AddVisibleBehaviour(unit);

            return unit;
        }

        public GameObject MakeGhostUnit(GameObject prefab)
        {
            GameObject unit = Object.Instantiate(prefab);
            unit.GetComponent<UnitBehaviour>().enabled = false;

            Shader shader = Resources.Load<Shader>("Ghost");
            unit.ApplyShaderRecursively(shader);
            unit.transform.position = 100 * Vector3.down;

            return unit;
        }

        private void AddMinimapIcon(GameObject unit, Color minimapColor)
        {
            var minimapIcon = GameObject.Instantiate(Resources.Load<GameObject>("MiniMapIcon"));
            minimapIcon.GetComponent<SpriteRenderer>().color = minimapColor;
            minimapIcon.transform.parent = unit.transform;
            minimapIcon.transform.localPosition = Vector3.zero;
        }

        private void AddVisibleBehaviour(GameObject unit)
        {
            var unitBehaviour = unit.GetComponent<UnitBehaviour>();
            VisibleBehavior vis = new VisibleBehavior(unit, unitBehaviour);
            unitBehaviour.VisibleBehavior = vis;

            // GameObjectEntity may simplify the creation of components, but it has some restrictions (only one component type per object, e.g. no multiple weapons..
            //var e = unitBehaviour.gameObject.GetComponent<GameObjectEntity>();
            //_session.EntityManager.AddComponentData(e.Entity, new Vision());
            
            unitBehaviour.Entity = _session.EntityManager.CreateEntity();
            VisionComponent v = new VisionComponent();
            v.max_spot_range = 800;

            _session.EntityManager
                .AddComponentData(unitBehaviour.Entity, v);
            _session.EntityManager
                .AddComponentData(unitBehaviour.Entity, new Position());
        }
    }

    public enum UnitType
    {
        Tank,
        Infantry,
        AFV,
        Arty
    }
}