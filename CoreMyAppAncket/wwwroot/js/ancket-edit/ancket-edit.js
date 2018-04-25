function Edit() {
    _this = this;
    this.ajaxAncketForm = "/Ancket/AncketForm";
    this.init = function () {
        $("#AncketForm").click(function () {
            $.ajax({
                type: "GET",
                url: _this.ajaxAncketForm,
                success: function (data) {
                    $("#divTableBody").append(data);
                }
            });
        });

        $(document).on("change", "div#divTableCell .form-control", function (e) {
            var keydown = $(e.target).attr('key');
            var iddown = $(e.target).attr('id');
            var typeno = $('#' + iddown + ' option:selected').val();

            $.ajax({
                type: "GET",
                url: "/Ancket/FormData",
                data: { typeno: typeno, key: keydown },
                datatype: "html",
                success: function (data) {
                    $('#Item_' + keydown).after().nextAll().remove();
                    $('#FormRow_' + keydown).append(data);

                },
                error: function () {
                    alert("Something went wrong in ddown.");
                }
            });
        });


        $(document).on("click", "#remove-line", function () {
            $('input:checkbox[name="CheckRow"]:checked').parent().parent().remove();
        });

        $(document).on("click", "#upbtn", function () {
            $('input:checkbox[name="CheckRow"]:checked').parent().parent().each(function (i, val) {
                var row = $(val);
                row.insertBefore(row.prev());
            });
        });

        $(document).on("click", "#downbtn", function () {
            var rows = $('input:checkbox[name="CheckRow"]:checked').parent().parent().get().reverse();
            $(rows).each(function (i, val) {
                var roww = $(val);
                roww.insertAfter(roww.next());
            });
        });

    };
}
var edit = null;
    $().ready(function () {
        edit = new Edit();
        edit.init();
    });