$(document).ready(function () {
    var opt =
        {
            url: function (phrase) {
                return '../Helper/Autocomplete';
            },
            getValue: function (data) {
                return data.Name;
            },
            ajaxSettings:
                {
                    dataType: 'json',
                    method: 'POST',
                    data: { dataType : 'json' }
                },
            preparePostData: function (data) {
                data.typeSearch = $('input:radio[name=typeSearch]:checked').val();
                data.prefix = $('#search_q').val();
                return data;
            },
            requestDelay: 400,
            list:
                {
                    onClickEvent: function ()
                    {
                        $('#EmeciSelected').val($("#search_q").getSelectedItemData().Value);
                        if ($('#search_q').val() != '')
                            $('#content_form').submit();
                    }
                }
        };
    $('#search_q').easyAutocomplete(opt);
    $('#search_q').attr('placeholder', '# Emeci');

    $('input:radio[name=typeSearch]').change(function () {
        $('#search_q').val('');

        if ($(this).val() == 'byname')
            $('#search_q').attr('placeholder', 'Nombre');
        else if ($(this).val() == 'bynumber')
            $('#search_q').attr('placeholder', '# Emeci');
    })

    $('#search_q').keypress(function (e) {
        if ($('input:radio[name=typeSearch]:checked').val() == 'bynumber')
        {
            //45 = guion
            if (e.which != 8 && e.which != 45 && isNaN(String.fromCharCode(e.which)))
                e.preventDefault();
        } else if ($('input:radio[name=typeSearch]:checked').val() == 'byname')
        {
            //32 = espacio
            if (!isNaN(String.fromCharCode(e.which)) && e.which != 32)
                e.preventDefault();
        }
    })
})