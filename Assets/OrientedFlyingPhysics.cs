﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientedFlyingPhysics : MonoBehaviour
{
    private void Awake()
    {
        entity = GetComponentInParent<StacsEntity>();
        entity.position = transform.localPosition;
        entity.altitude = entity.desiredAltitude = transform.localPosition.y;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public StacsEntity entity;
    public Vector3 eulerRotation = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        //speed
        if (Utils.ApproximatelyEqual(entity.speed, entity.desiredSpeed)) {
            ;
        } else if (entity.speed < entity.desiredSpeed) {
            entity.speed = entity.speed + entity.acceleration * Time.deltaTime;
        } else if (entity.speed > entity.desiredSpeed) {
            entity.speed = entity.speed - entity.acceleration * Time.deltaTime;
        }
        entity.speed = Utils.Clamp(entity.speed, entity.minSpeed, entity.maxSpeed);

        //altitude
        if(Utils.ApproximatelyEqual(entity.altitude, entity.desiredAltitude)) {
            ;
        } else if (entity.altitude < entity.desiredAltitude) {
            entity.altitude += entity.climbRate * Time.deltaTime;
        } else if (entity.altitude > entity.desiredAltitude) {
            entity.altitude -= entity.climbRate * Time.deltaTime;
        }
        entity.altitude = Utils.Clamp(entity.altitude, entity.minAltitude, entity.maxAltitude);

        //heading
        if (Utils.ApproximatelyEqualAngle(entity.heading, entity.desiredHeading)) {
            ;
        } else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) > Utils.ANGLE_EPSILON) {
            entity.heading += entity.turnRate * Time.deltaTime;
        } else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) < Utils.ANGLE_EPSILON) {
            entity.heading -= entity.turnRate * Time.deltaTime;
        }
        entity.heading = Utils.Degrees360(entity.heading);

        //
        entity.velocity.x = Mathf.Sin(entity.heading * Mathf.Deg2Rad) * entity.speed;
        entity.velocity.y = 0;
        entity.velocity.z = Mathf.Cos(entity.heading * Mathf.Deg2Rad) * entity.speed;

        entity.position = entity.position + entity.velocity * Time.deltaTime;
        entity.position.y = entity.altitude;
        transform.localPosition = entity.position;

        eulerRotation.y = entity.heading;
        transform.localEulerAngles = eulerRotation;
    }



}
