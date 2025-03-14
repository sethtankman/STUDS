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
    public GameObject easterObjects;
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
            { "Christmas", (new DateTime(currentYear, 12, 25), christmasObjects) },
            { "Easter", (GetEasterDate(currentYear), easterObjects) }
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

    private static DateTime GetEasterDate(int year)
    {
        // Computus algorithm to calculate Easter Sunday
        int a = year % 19;
        int b = year / 100;
        int c = year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = ((h + l - 7 * m + 114) % 31) + 1;
        return new DateTime(year, month, day);
    }
}
