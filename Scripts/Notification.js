document.addEventListener("DOMContentLoaded", function(){

    function setCookie(cname, cvalue, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays*24*60*60*1000));
        var expires = "expires="+ d.toUTCString();
        document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
    }

    function getCookie(cname) {
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for(var i = 0; i <ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }

    /**
        * Main starts here
        */
    const cookieName = "JfcPortalCookieAgreement";
    const accepted = "accepted";
    const expireDays = 3000;

	const el = document.getElementsByClassName("footer-notification");
	if (!el) {
		console.info("Do nothing. Footer notification does not exists");
		return;
	}

    const notificationEl = el[0];
    const agreement = getCookie(cookieName);
    if (agreement === accepted) {
        console.info("Cookie is accepted.");
        notificationEl.style.display = "none";
        return;
    }
    notificationEl.style.display = "block";

    /**
        * Actions
        */
    document.getElementById("cookies-accept").addEventListener('click', function (ev) {
        ev.preventDefault();
        setCookie(cookieName, accepted, expireDays);
        notificationEl.style.display = "none";
    });

});
