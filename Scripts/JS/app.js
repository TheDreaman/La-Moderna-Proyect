$(document).ready(function() {

    $('#checkin').datepicker({
        language: "es",
        todayBtn: "linked",
        clearBtn: true,
        format: "yyyy/mm/dd",
        multidate: false,
        todayHighlight: true

    });    

    $('#checkout').datepicker({
        language: "es",
        todayBtn: "linked",
        clearBtn: true,
        format: "yyyy/mm/dd",
        multidate: false,
        todayHighlight: true

    });

});