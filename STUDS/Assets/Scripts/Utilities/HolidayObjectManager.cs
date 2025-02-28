using UnityEngine;
using System;
using System.Collections.Generic;

public class HolidayObjectManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool enableHolidays = true;
    [SerializeField] private bool debugHolidays = false;
    [SerializeField] private int debugMonth;
    [SerializeField] private int debugDay;

    [Header("Holiday Objects")]
    public GameObject newYearsObjects;
    public GameObject valentinesObjects;
    public GameObject stPatricksObjects;
    public GameObject independenceObjects;
    public GameObject halloweenObjects;
    public GameObject thanksgivingObjects;
    public GameObject christmasObjects;

    private Dictionary<string, (DateTime date, GameObject holidayObject)> holidays;

    void Start()
    {
        InitializeHolidays();

        if (enableHolidays)
        {
            UpdateHolidayObjects();
        }
        else
        {
            DisableAllHolidayObjects();
        }

        if (debugHolidays)
        {
            DebugHolidayObjects();
        }
    }

    void InitializeHolidays()
    {
        int currentYear = DateTime.Now.Year;
        holidays = new Dictionary<string, (DateTime, GameObject)>
        {
            { "New Year's Eve", (new DateTime(currentYear, 12, 31), newYearsObjects) },
            { "New Year's Day", (new DateTime(currentYear, 1, 1), newYearsObjects) },
            { "Valentine's Day", (new DateTime(currentYear, 2, 14), valentinesObjects) },
            { "St. Patrick's Day", (new DateTime(currentYear, 3, 17), stPatricksObjects) },
            { "Independence Day", (new DateTime(currentYear, 7, 4), independenceObjects) },
            { "Halloween", (new DateTime(currentYear, 10, 31), halloweenObjects) },
            { "Thanksgiving", (GetThanksgivingDate(currentYear), thanksgivingObjects) },
            { "Christmas", (new DateTime(currentYear, 12, 25), christmasObjects) }
        };
    }

    void UpdateHolidayObjects()
    {
        DateTime today = DateTime.Today;

        foreach (var holiday in holidays)
        {
            DateTime holidayDate = holiday.Value.date;
            DateTime startDate = holidayDate.AddDays(-7);
            DateTime endDate = holidayDate.AddDays(1);
            bool isActive = today >= startDate && today <= endDate;

            holiday.Value.holidayObject?.SetActive(isActive);
        }
    }

    void DebugHolidayObjects()
    {
        DateTime testDate = new DateTime(DateTime.Now.Year, debugMonth, debugDay);
        
        foreach (var holiday in holidays)
        {
            DateTime holidayDate = holiday.Value.date;
            DateTime startDate = holidayDate.AddDays(-7);
            DateTime endDate = holidayDate.AddDays(1);
            bool isActive = testDate >= startDate && testDate <= endDate;

            holiday.Value.holidayObject?.SetActive(isActive);
        }
    }

    void DisableAllHolidayObjects()
    {
        foreach (var holiday in holidays)
        {
            holiday.Value.holidayObject?.SetActive(false);
        }
    }

    private static DateTime GetThanksgivingDate(int year)
    {
        DateTime firstDayOfNovember = new DateTime(year, 11, 1);
        int daysUntilThursday = (DayOfWeek.Thursday - firstDayOfNovember.DayOfWeek + 7) % 7;
        return firstDayOfNovember.AddDays(daysUntilThursday + (3 * 7));
    }
}
