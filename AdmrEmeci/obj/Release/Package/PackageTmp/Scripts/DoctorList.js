$(document).ready(function () {

    var options =
        {
            url: function (Prefix) {
                return 'AutoComplete';
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
                        if ($('#CardNumber').val() != '')
                            $('#xpost').submit();
                    }
                }
        };

    $('#CardNumber').easyAutocomplete(options);
});