/*
---------------------------------------------------
    Code written by Andres Correa for AgenciaUAO
    2024
---------------------------------------------------
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] //See all class attributes in inspector if the class is a component used in a script attached to it
public class TreatmentType
{
    public enum TreatmentKind{ //for comparing preferred treatment in Kit Construction level
        Drops,
        Shots,
        NoTreatment //PEANUTS AND STRAWBERRIES ONLY
    }
    [Tooltip("'NoTreatment' is reserved for peanut and strawberry only, since it's food")]
    public TreatmentKind treatmentAdministration;
    public AllergiesManager.Allergies treatmentToAllergy;
}
