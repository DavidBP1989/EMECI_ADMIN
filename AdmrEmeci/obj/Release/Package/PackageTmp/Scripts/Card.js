$(document).ready(function () {

    var options =
        {
            url: function (Prefix) {
                return 'Card/AutoComplete';
            },
            getValue: function (element) {
                return element.Emeci;
            },
            ajaxSettings:
                {
                    dataType: 'json',
                    method: 'POST',
                    data: { dataType: 'json' }
                },
            preparePostData: function (data) {
                data.Prefix = $('#CardNumber').val();
                return data;
            },
            requestDelay: 400,
            list:
                {
                    onClickEvent: function () {
                        
                        $('#xpost').submit();
                    }
                }
        };

    $('#CardNumber').easyAutocomplete(options);
});

function DownloadImage(Emeci)
{
    $.ajax({
        url: 'Card/Download',
        method: 'POST',
        data: { CardNumber: Emeci },
        success: function ()
        {
            window.location = '@Url.Action("Download", "Card", new { studentId = 123 })';
        }
    });
}