namespace Trojan.Models;

using System;
using Trojan.ViewModels;

public class CalendarEvent : ObservableObject
{
    private int _id;
    public int Id { get => _id; set => SetProperty(ref _id, value); }

    private string _title = string.Empty;
    public string Title { get => _title; set => SetProperty(ref _title, value); }

    private string _description = string.Empty;
    public string Description { get => _description; set => SetProperty(ref _description, value); }

    private DateTime _startDateTime;
    public DateTime StartDateTime { get => _startDateTime; set => SetProperty(ref _startDateTime, value); }

    private DateTime _endDateTime;
    public DateTime EndDateTime { get => _endDateTime; set => SetProperty(ref _endDateTime, value); }

    private string _location = string.Empty;
    public string Location { get => _location; set => SetProperty(ref _location, value); }

    private string _colorHex = "#A13599";
    public string ColorHex { get => _colorHex; set => SetProperty(ref _colorHex, value); }
}