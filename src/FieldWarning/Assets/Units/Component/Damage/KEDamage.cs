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

namespace PFW.Units.Component.Damage
{
    class KEDamage:Damage
    {
        private DamageData.KineticData _keData;
        private float _distance;

        public KEDamage(DamageData.KineticData data, DamageData.Target target, float distance)
            : base(DamageType.KE, target)
        {
            _keData = data;
            _distance = distance;
        }

        public override DamageData.Target CalculateDamage()
        {
            DamageData.Target finalState = this.CurrentTarget;
            DamageData.KineticData ke = _keData;

            Logger.LogDamage($"Initial KE dmg is {ke.Power}");

            // Calculate attenuation of air friction
            ke.Power = CalculateKEAttenuationSimple(
                ke.Power,
                _distance,
                ke.Friction
            );

            Logger.LogDamage($"Attenuated KE dmg is {ke.Power}");

            // Hack because the formulas below do not work, remove when fixed:
            finalState.Health -= ke.Power;
            return finalState;

            //if (finalState.EraData.Value > 0.0f) {
            //    // Calculate effects of ERA
            //    float finalEra = Math.Max(
            //        0.0f,
            //        finalState.EraData.Value - ke.Power * finalState.EraData.KEFractionMultiplier
            //    );
            //    finalState.EraData.Value = finalEra;

            //    ke.Power = CalculatePostEraPower(
            //        ke.Power,
            //        finalState.EraData.KEFractionMultiplier
            //    );
            //    Logger.LogDamage($"Post ERA KE dmg is {ke.Power}");
            //}

            //// Armor degradation
            //float finalArmor = Math.Max(
            //    0.0f,
            //    finalState.Armor - (ke.Power / finalState.Armor) * ke.Degradation
            //);
            //finalState.Armor = finalArmor;

            //// Calculate final damage
            //float finalDamage = Math.Max(
            //    0.0f,
            //    (ke.Power - finalState.Armor) * ke.HealthDamageFactor
            //);
            //Logger.LogDamage($"Final KE dmg is {finalDamage}");
            //float finalHealth = Math.Max(
            //    0.0f,
            //    finalState.Health - finalDamage
            //);
            //finalState.Health = finalHealth;

            //return finalState;
        }


        private static float CalculateKEAttenuationSimple(float power, float distance, float friction)
        {
            return  (float)Math.Exp(-friction * distance) * power;
        }

        private static float CalculatePostEraPower(float power, float eraFractionMultiplier)
        {
            return power * (1 - eraFractionMultiplier);
        }
    }
}