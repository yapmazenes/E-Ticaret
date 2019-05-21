/** Used Only For Touch Devices **/
function Sil(ActionName, Name, ID, sonuc,rePlace) {
    alert("geldi");
    if (sonuc) {
        var name = Name;
        alert(name);
        alert(ActionName);
        alert(ID);
        $.ajax({
            type: "POST",
            url: "/Admin/" + ActionName + "/" + ID,
            success: function () {
                alert("Silindi")
                window.location.replace("/Admin/" + rePlace);
            },
            error: function () {
                alert("SÝLÝNEMEDÝ");

            }
        });

    }


}