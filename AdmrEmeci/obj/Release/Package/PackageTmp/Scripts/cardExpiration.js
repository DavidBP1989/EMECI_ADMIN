
function SaveEmeci(element, emeci, status)
{
    //sessionStorage.clear();
    console.log(status);
    var isCheck = $(element)[0].checked;
    if (sessionStorage.getItem('emeci_selected') == null)
        sessionStorage.setItem('emeci_selected', JSON.stringify([]));

    if (sessionStorage.getItem('emeci_selected') != null)
    {
        var arrayEmecis = JSON.parse(sessionStorage.getItem('emeci_selected'));
        var index = arrayEmecis.indexOf(emeci);
        if (index < 0 && isCheck)
        {
            arrayEmecis.push(emeci);
            $(element).parent().parent().addClass('cardSelected');
            $('#showEmecis').show();
        }
        else if (!isCheck)
        {
            if (index >= 0)
            {
                arrayEmecis.splice(index, 1);
                $(element).parent().parent().removeClass('cardSelected');
            }
        }

        sessionStorage.setItem('emeci_selected', JSON.stringify(arrayEmecis));
    }

    EmeciSelected();
    //console.log(sessionStorage.getItem('emeci_selected'));
}

function Renovation()
{
    if (sessionStorage.getItem('emeci_selected') != null)
    {
        var arrayEmecis = JSON.parse(sessionStorage.getItem('emeci_selected'));
        if (arrayEmecis.length > 0)
        {
            $.ajax({
                url: 'CardRenovation',
                type: 'POST',
                data: 'emecis=' + arrayEmecis.join(),
                success: function ()
                {
                    sessionStorage.clear();
                    window.location = '/Card/CardExpiration'
                }
            })
        }
    }
}

function EmeciSelected()
{
    if (sessionStorage.getItem('emeci_selected') != null)
    {
        var arrayEmecis = JSON.parse(sessionStorage.getItem('emeci_selected'));
        if (arrayEmecis.length > 0)
        {
            $('#listEmeci li').remove();
            $('#showEmecis').show();
            arrayEmecis.map(function (emeci) {
                $('#listEmeci').append('<li>' + emeci + '</li>');
            })
        } else
        {
            $('#listEmeci li').remove();
            $('#showEmecis').hide();
        }
    }
}

$(document).ready(function () {
    $('#showEmecis').hide();
    if (sessionStorage.getItem('emeci_selected') != null)
    {
        var arrayEmecis = JSON.parse(sessionStorage.getItem('emeci_selected'));
        arrayEmecis.map(function (emeci) {
            $('#row_' + emeci).prop('checked', true);
            $('#row_' + emeci).parent().parent().css('background', 'antiquewhite');
        })
    }

    EmeciSelected();
})