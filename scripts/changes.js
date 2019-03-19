$(document).ready(function () {

    $(document).on("click", ".change-pas", function () {
        $(".change-header").empty();
        $(".change-header").append(`
                 <h4 class ="modal-title">cahnge password</h4>
                <button type="button" class ="close" data-dismiss="modal">&times; </button>
            `);
        $(".change-body").empty();
        $(".change-body").append(`
              <form>
                 <div class ="form-group">
                    <label for="ex-pas">password </label>
                    <input id="ex-pas"  type="password" class ="before-pas form-control" name="name" value="" />
                </div>
                  <div class ="form-group">
                    <label for="new-pas">new password </label>
                    <input id="new-pas" type="password" class ="before-pas form-control" name="pas" value="" />
                </div>
                <div class ="form-group">
                    <label for="con-pas">confim password </label>
                    <input id="con-pas" type="password" class ="rep-after-pas form-control" name="con" value="" />
                </div>
                 <input type="submit" class ="send-pas form-control" name="name" value="confirm" />
               </form>
            `)
    });

    $(document).on('click', '.send-pas', function () {
        let pasEx=new String($("#ex-pas").val())
        let pas=new String($("#new-pas").val())
        let pasConf = new String($("#con-pas").val())
        if ((pasEx != "" || pasEx != undefined) && (pasConf != "" || pasConf != undefined) && pas != "" || pas != undefined)
        $.ajax({
            type: "post",
            url: "/Profile/ChangeExPas/",
            data: { pasEx: pasEx, pas: pas, pasConf: pasConf },
            success: function (r) {
                if (r.error != "success") {
                    let mes = r.error;
                    $(".err-mes").empty();
                    $(".err-mes").addClass("text-danger")
                    $(".err-mes").append(`${mes}`);
                } else {
                    $(".err-mes").empty();
                    $(".err-mes").removeClass("text-danger").addClass("text-success");
                    $("#ex-pas").val("")
                    $("#new-pas").val("")
                    $("#con-pas").val("")
                    $(".err-mes").append("Succesfuly changed");
                }
            }
        });
    });
    $(document).on('click', '.dropdown-menu', function (e) {
        $(this).css("z-index","100")
        e.stopPropagation();
    });
    $(document).on('click', '#acc-set', function () {
        $(this).show();
        $('#sub-men').slideDown();
        alert("5")
    });
    $(document).on('click', '.redact-profile', function () {
        $(".change-header").empty();
        $(".change-header").append(`
                 <h4 class ="modal-title">Edit Profile</h4>
                <button type="button" class ="close" data-dismiss="modal">&times; </button>
            `);
        $(".change-body").empty();
        $(".change-body").append(`
              <form>
                <div class ="form-group">
                    <label for="login">login</label>
                    <input id="login" type="text" class ="form-control" name="con" value="" />
                </div>
                <div class ="form-group">
                    <label for="name">name</label>
                    <input id="name"  type="text" class ="form-control" name="name" value="" />
                </div>
                  <div class ="form-group">
                    <label for="surname">surname</label>
                    <input id="surname" type="text" class ="form-control" name="pas" value="" />
                </div>
                <div class ="form-group">
                    <label for="age">age</label>
                    <input id="age" type="number" class ="form-control" name="con" value="" />
                </div>
                 <input type="submit" class ="red-conf form-control" name="name" value="confirm" />
               </form>
            `)
    });
    $(document).on('click', '.red-conf', function () {
        let login = new String($("#login").val());
        let name = new String($("#name").val());
        let surname = new String($("#surname").val());
        let age = new Number($("#age").val());
        if (login == "") {
            login = "0";
        } if (name == "") {
            name="0"
        } if (surname == "") {
            surname = "0";
        } if (age == "") {
            age = "0";
        }
            $.ajax({
                type: "post",
                url: "/Profile/Redact/",
                data: { name: name, surname: surname, login:login,age:age },
                success: function (r) {
                    if (r.error != "success") {
                        let mes = r.error;
                        $(".err-mes").empty();
                        $(".err-mes").addClass("text-danger")
                        $(".err-mes").append(`${mes}`);
                    } else {
                        $(".err-mes").empty();
                        $(".err-mes").removeClass("text-danger").addClass("text-success");
                        $("#name").val("")
                        $("#surnam").val("")
                        $("#login").val("")
                        $("#age").val("")
                        $(".err-mes").append("Succesfuly changed");
                    }
                }
            });
    });
});