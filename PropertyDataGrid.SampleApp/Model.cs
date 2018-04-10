﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Media;
using PropertyDataGrid.Utilities;

namespace PropertyDataGrid.SampleApp
{
    public class Customer : DictionaryObject
    {
        public Customer()
        {
            Id = Guid.NewGuid();
            ListOfStrings = new List<string>();
            ListOfStrings.Add("string 1");
            ListOfStrings.Add("string 2");

            ArrayOfStrings = ListOfStrings.ToArray();
            CreationDateAndTime = DateTime.Now;
            Description = "press button to edit...";
            ByteArray1 = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            WebSite = "http://www.softfluent.com";
            Status = Status.Valid;
            Addresses = new ObservableCollection<Address> { new Address { Line1 = "2018 156th Avenue NE", City = "Bellevue", State = "WA", ZipCode = 98007, Country = "USA" } };
            DaysOfWeek = DaysOfWeek.WeekDays;
            PercentageOfSatisfaction = 50;
            PreferredColorName = "DodgerBlue";
            PreferredFont = Fonts.SystemFontFamilies.FirstOrDefault(f => string.Equals(f.Source, "Consolas", StringComparison.OrdinalIgnoreCase));
            SampleNullableBooleanDropDownList = false;
            SampleBooleanDropDownList = true;
            MultiEnumString = "First, Second";
            SubObject = Address.Parse("1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA");
        }

        [DisplayName("Guid (see menu on right-click)")]
        public Guid Id { get => DictionaryObjectGetPropertyValue<Guid>(); set => DictionaryObjectSetPropertyValue(value); }

        //[ReadOnly(true)]
        [Category("Dates and Times")]
        public DateTime CreationDateAndTime { get => DictionaryObjectGetPropertyValue<DateTime>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Sub Object (Address)")]
        public Address SubObject
        {
            get => DictionaryObjectGetPropertyValue<Address>();
            set
            {
                // because it's a sub object we want to update the property grid
                // when inner properties change
                var so = SubObject;
                if (so != null)
                {
                    so.PropertyChanged -= OnSubObjectPropertyChanged;
                }

                if (DictionaryObjectSetPropertyValue(value))
                {
                    so = SubObject;
                    if (so != null)
                    {
                        so.PropertyChanged += OnSubObjectPropertyChanged;
                    }

                    // these two properties are coupled
                    OnPropertyChanged(nameof(SubObjectAsObject));
                }
            }
        }

        private void OnSubObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(this, nameof(SubObject));
            OnPropertyChanged(this, nameof(SubObjectAsObject));
        }

        [DisplayName("Sub Object (Address as Object)")]
        public Address SubObjectAsObject { get => SubObject; set => SubObject = value; }

        public string FirstName { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public string LastName { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }

        [Category("Dates and Times")]
        public DateTime DateOfBirth { get => DictionaryObjectGetPropertyValue<DateTime>(); set => DictionaryObjectSetPropertyValue(value); }

        [Category("Enums")]
        public Gender Gender { get => DictionaryObjectGetPropertyValue<Gender>(); set => DictionaryObjectSetPropertyValue(value); }

        [Category("Enums")]
        public Status Status
        {
            get => DictionaryObjectGetPropertyValue<Status>();
            set
            {
                if (DictionaryObjectSetPropertyValue(value))
                {
                    OnPropertyChanged(nameof(StatusColor));
                    OnPropertyChanged(nameof(StatusColorString));
                }
            }
        }

        [DisplayName("Status (colored enum)")]
        [ReadOnly(true)]
        [Category("Enums")]
        public Status StatusColor { get => Status; set => Status = value; }

        [DisplayName("Status (enum as string list)")]
        [Category("Enums")]
        public string StatusColorString { get => Status.ToString(); set => Status = (Status)Enum.Parse(typeof(Status), value); }

        [Category("Enums")]
        public string MultiEnumString { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }

        [Category("Enums")]
        public string MultiEnumStringWithDisplay { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }

        [Category("Dates and Times")]
        [Description("This is the timespan tooltip")]
        public TimeSpan TimeSpan { get => DictionaryObjectGetPropertyValue<TimeSpan>(); set => DictionaryObjectSetPropertyValue(value); }

        [Category("Security")]
        [DisplayName("Password (SecureString)")]
        public SecureString Password
        {
            get => DictionaryObjectGetPropertyValue<SecureString>();
            set
            {
                if (DictionaryObjectSetPropertyValue(value))
                {
                    OnPropertyChanged(nameof(PasswordString));
                }
            }
        }

        [Category("Security")]
        [DisplayName("Password (String)")]
        public string PasswordString => Password.ConvertToUnsecureString();

        [Browsable(false)]
        public string NotBrowsable { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Description (multi-line)")]
        public string Description { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }

        [ReadOnly(true)]
        [DisplayName("Byte Array (hex format)")]
        public byte[] ByteArray1 { get => DictionaryObjectGetPropertyValue<byte[]>(); set => DictionaryObjectSetPropertyValue(value); }

        [ReadOnly(true)]
        [DisplayName("Byte Array (press button for hex dump)")]
        public byte[] ByteArray2 { get => ByteArray1; set => ByteArray1 = value; }

        [DisplayName("Web Site (custom sort order)")]
        public string WebSite { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }

        [Category("Collections")]
        public string[] ArrayOfStrings { get => DictionaryObjectGetPropertyValue<string[]>(); set => DictionaryObjectSetPropertyValue(value); }

        [Category("Collections")]
        public List<string> ListOfStrings { get => DictionaryObjectGetPropertyValue<List<string>>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Addresses (custom editor)")]
        [Category("Collections")]
        public ObservableCollection<Address> Addresses { get => DictionaryObjectGetPropertyValue<ObservableCollection<Address>>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Days Of Week (multi-valued enum)")]
        [Category("Enums")]
        public DaysOfWeek DaysOfWeek { get => DictionaryObjectGetPropertyValue<DaysOfWeek>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Percentage Of Satisfaction (int)")]
        public int PercentageOfSatisfactionInt { get => DictionaryObjectGetPropertyValue(0, nameof(PercentageOfSatisfaction)); set => DictionaryObjectSetPropertyValue(value, nameof(PercentageOfSatisfaction)); }

        [DisplayName("Percentage Of Satisfaction (double)")]
        public double PercentageOfSatisfaction
        {
            get => DictionaryObjectGetPropertyValue<double>();
            set
            {
                if (DictionaryObjectSetPropertyValue(value))
                {
                    OnPropertyChanged(nameof(PercentageOfSatisfactionInt));
                }
            }
        }

        [DisplayName("Preferred Color Name (custom editor)")]
        public string PreferredColorName { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Preferred Font (custom editor)")]
        public FontFamily PreferredFont { get => DictionaryObjectGetPropertyValue<FontFamily>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Point (auto type converter)")]
        public Point Point { get => DictionaryObjectGetPropertyValue<Point>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Nullable Int32 (supports empty string)")]
        public int? NullableInt32 { get => DictionaryObjectGetPropertyValue<int?>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Boolean (Checkbox)")]
        [Category("Booleans")]
        public bool SampleBoolean { get => DictionaryObjectGetPropertyValue<bool>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Boolean (Checkbox three states)")]
        [Category("Booleans")]
        public bool? SampleNullableBoolean { get => DictionaryObjectGetPropertyValue<bool?>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Boolean (DropDownList)")]
        [Category("Booleans")]
        public bool SampleBooleanDropDownList { get => DictionaryObjectGetPropertyValue<bool>(); set => DictionaryObjectSetPropertyValue(value); }

        [DisplayName("Boolean (DropDownList 3 states)")]
        [Category("Booleans")]
        public bool? SampleNullableBooleanDropDownList { get => DictionaryObjectGetPropertyValue<bool?>(); set => DictionaryObjectSetPropertyValue(value); }

        public double DoubleValue { get => DictionaryObjectGetPropertyValue<double>(); set => DictionaryObjectSetPropertyValue(value); }

        protected override IEnumerable DictionaryObjectGetErrors(string propertyName)
        {
            if (propertyName == null || propertyName == nameof(DoubleValue))
            {
                if (DoubleValue < 10)
                {
                    yield return nameof(DoubleValue) + " must be >= 10";
                }
            }
        }
    }

    [TypeConverter(typeof(AddressConverter))]
    public class Address : DictionaryObject
    {
        public string Line1 { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public string Line2 { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public int? ZipCode { get => DictionaryObjectGetPropertyValue<int?>(); set => DictionaryObjectSetPropertyValue(value); }
        public string City { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public string State { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public string Country { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }

        // poor man's one line comma separated USA postal address parser
        public static Address Parse(string text)
        {
            var address = new Address();
            if (text != null)
            {
                string[] split = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 0)
                {
                    int zip = 0;
                    int index = -1;
                    string state = null;
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (TryFindStateZip(split[i], out state, out zip))
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index < 0)
                    {
                        address.DistributeOverProperties(split, 0, int.MaxValue, nameof(Line1), nameof(Line2), nameof(City), nameof(Country));
                    }
                    else
                    {
                        address.ZipCode = zip;
                        address.State = state;
                        address.DistributeOverProperties(split, 0, index, nameof(Line1), nameof(Line2), nameof(City));
                        if (string.IsNullOrWhiteSpace(address.City) && address.Line2 != null)
                        {
                            address.City = address.Line2;
                            address.Line2 = null;
                        }
                        address.DistributeOverProperties(split, index + 1, int.MaxValue, nameof(Country));
                    }
                }
            }
            return address;
        }

        private static bool TryFindStateZip(string text, out string state, out int zip)
        {
            zip = 0;
            state = null;
            string zipText = text;
            int pos = text.LastIndexOfAny(new[] { ' ' });
            if (pos >= 0)
            {
                zipText = text.Substring(pos + 1).Trim();
            }

            if (!int.TryParse(zipText, out zip) || zip <= 0)
                return false;

            state = text.Substring(0, pos).Trim();
            return true;
        }

        private void DistributeOverProperties(string[] split, int offset, int max, params string[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if ((offset + i) >= split.Length || (offset + i) >= max)
                    return;

                string s = split[offset + i].Trim();
                if (s.Length == 0)
                    continue;

                DictionaryObjectSetPropertyValue((object)s, properties[i]);
            }
        }

        public override string ToString()
        {
            const string sep = ", ";
            var sb = new StringBuilder();
            AppendJoin(sb, Line1, string.Empty);
            AppendJoin(sb, Line2, sep);
            AppendJoin(sb, City, sep);
            AppendJoin(sb, State, sep);
            if (ZipCode.HasValue)
            {
                AppendJoin(sb, ZipCode.Value.ToString(), " ");
            }
            AppendJoin(sb, Country, sep);
            return sb.ToString();
        }

        private static void AppendJoin(StringBuilder sb, string value, string sep)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            string s = sb.ToString();
            if (!s.EndsWith(" ") && !s.EndsWith(",") && !s.EndsWith(Environment.NewLine))
            {
                sb.Append(sep);
            }
            sb.Append(value);
        }
    }

    [Flags]
    public enum DaysOfWeek
    {
        NoDay = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
        WeekDays = Monday | Tuesday | Wednesday | Thursday | Friday
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum Status
    {
        Unknown,
        Invalid,
        Valid
    }

    public class AddressConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
                return Address.Parse(s);

            return base.ConvertFrom(context, culture, value);
        }
    }

    public class PointConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                string[] v = s.Split(new[] { ';' });
                return new Point(int.Parse(v[0]), int.Parse(v[1]));
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ((Point)value).X + ";" + ((Point)value).Y;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(PointConverter))]
    public struct Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y)
            : this()
        {
            X = x;
            Y = y;
        }
    }
}
