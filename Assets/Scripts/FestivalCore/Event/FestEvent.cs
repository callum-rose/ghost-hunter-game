using UnityEngine;
using System;

public class FestEvent
{
    public string Name { get; private set; }
    public EventCategory Category { get; private set; }
    public Vector2 Location { get; private set; }
    public DateTime Date { get; private set; }

    public FestEvent(string name, EventCategory category, Vector2 location, DateTime date)
    {
        Name = name;
        Category = category;
        Location = location;
        Date = date;
    }

    public enum EventCategory
    {
        MUSIC,
        COMEDY,
        EDUCATION
    }
}