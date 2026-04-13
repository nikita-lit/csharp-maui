using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls.Shapes;

namespace Example;

public partial class Contacts : ContentPage
{
    private TableView _tabelview;
    private SwitchCell _sc;
    private ImageCell _ic;
    private ViewCell _photoCell;
    private TableSection _fotosection;

    private EntryCell _phone;
    private EntryCell _message;
    private EntryCell _email;

    private byte[] _imageBytes;

    public Contacts()
    {
        _sc = new SwitchCell { 
            Text = "Näita pilti" 
        };
        _sc.OnChanged += Sc_OnChanged;

        _ic = new ImageCell {
            ImageSource = ImageSource.FromFile("bob.jpg"),
            Text = "Foto",
            Detail = "Ta on sõber!",
        };

        var photoBut = new Button { Text="Vali Foto", WidthRequest = 120 };
        photoBut.Clicked += ValiFoto_Clicked;

        _photoCell = new ViewCell {
            View = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                Spacing = 10,
                Children =
                {
                    photoBut,
                }
            }
        };

        _fotosection = new TableSection();

        _phone = new EntryCell {
            Label = "Telefon",
            Placeholder = "Sisesta tel. number",
            Keyboard = Keyboard.Telephone
        };

        _message = new EntryCell
        {
            Label = "Sõnum",
            Placeholder = "Sisesta sõnum"
        };

        _email = new EntryCell {
            Label = "Email",
            Placeholder = "Sisesta email",
            Keyboard = Keyboard.Email
        };
        
        var emailBut = new Button { Text="Saada Email", WidthRequest = 120 };
        emailBut.Clicked += Saada_email_Clicked;        

        var phoneBut = new Button { Text="Helista", WidthRequest = 120 };
        phoneBut.Clicked += Helista_Clicked;

        var smsBut = new Button { Text="Saada SMS", WidthRequest = 120 };
        smsBut.Clicked += Saada_sms_Clicked;

        _tabelview = new TableView
        {
            Intent = TableIntent.Form,
            Root = new TableRoot
            {
                new TableSection("Kontaktandmed:"),

                _fotosection,

                new TableSection()
                {
                    _sc,
                    _phone,
                    _email,
                    _message,
                },

                new TableSection()
                {
                    new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            VerticalOptions = LayoutOptions.Start,
                            HorizontalOptions = LayoutOptions.Center,
                            Spacing = 10,
                            Children =
                            {
                                phoneBut,
                                smsBut,
                                emailBut,
                            }
                        }
                    },
                }
            }
        };

        Content = _tabelview;
    }

    private async void ValiFoto_Clicked(object? sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();     
            if (photo != null)
            {
                using var stream = await photo.OpenReadAsync();
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    _imageBytes = ms.ToArray();
                }

                _ic.ImageSource = ImageSource.FromStream(() => new MemoryStream(_imageBytes));
            }
        }
        else
            await DisplayAlertAsync("Viga", "Galerii ei ole kättesaadav", "OK");
    }

    private void Sc_OnChanged(object? sender, ToggledEventArgs e)
    {
        _fotosection.Clear();

        if (e.Value)
        {
            _fotosection.Title = "Foto:";
            _fotosection.Add(_ic);
            _fotosection.Add(_photoCell);
            _sc.Text = "Peida";
        }
        else
        {
            _fotosection.Title = "";
            _sc.Text = "Näita veel";
        }
    }

    private async void Helista_Clicked(object? sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_phone.Text))
            PhoneDialer.Open(_phone.Text);
        else
            await DisplayAlertAsync("SMS", "Palun sisestage telefoninumber!", "OK");
    }

    private async void Saada_sms_Clicked(object? sender, EventArgs e)
    {
        string phone = _phone.Text;
        if (string.IsNullOrWhiteSpace(phone))
        {
            await DisplayAlertAsync("SMS", "Palun sisestage telefoninumber!", "OK");
            return;
        }

        var message = _message.Text;
        SmsMessage sms = new(message, phone);

        if (phone != null && Sms.Default.IsComposeSupported)
            await Sms.Default.ComposeAsync(sms);
    }

    private async void Saada_email_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_email.Text))
        {
            await DisplayAlertAsync("E-post", "Palun sisestage e-posti aadress!", "OK");
            return;
        }

        var message = _message.Text;
        EmailMessage e_mail = new EmailMessage
        {
            Subject = _email.Text,
            Body = message,
            BodyFormat = EmailBodyFormat.PlainText,
            To = [_email.Text]
        };

        if (Email.Default.IsComposeSupported)
            await Email.Default.ComposeAsync(e_mail);
        else
            await DisplayAlertAsync("Viga", "E-maili saatmine pole selles seadmes toetatud", "OK");
    }
}