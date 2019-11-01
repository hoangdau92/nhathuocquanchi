jQuery(function () {

});

function mergeColumn(idtable) {
    var flag = 0;
    jQuery('#' + idtable + ' > tbody  > tr').each(function () {
        var _class = jQuery(this).attr('class');
        var td = jQuery(this).find('td[name=mergeColumn]').eq(0);

        var id = _class.split('_')[1];
        if (id != flag) {
            td.attr('rowspan', jQuery('.' + _class).length).css('vertical-align', 'middle');
            flag = id;
        }
        else {
            td.remove();
        }
    });
}