document.addEventListener("DOMContentLoaded", () => {

    const header = document.getElementById("siteHeader");

    const mobileButton =
        document.getElementById("mobileMenuButton");

    const mobileNavigation =
        document.getElementById("mobileNavigation");


    /* =====================================================
       HEADER AO ROLAR
    ====================================================== */

    const handleScroll = () => {

        if (!header) return;

        if (window.scrollY > 30) {

            header.classList.add("scrolled");

        } else {

            header.classList.remove("scrolled");

        }

    };


    window.addEventListener(
        "scroll",
        handleScroll,
        { passive: true }
    );


    handleScroll();


    /* =====================================================
       MENU MOBILE
    ====================================================== */

    if (mobileButton && mobileNavigation) {

        mobileButton.addEventListener("click", () => {

            const isOpen =
                mobileNavigation.classList.toggle("open");

            mobileButton.setAttribute(
                "aria-expanded",
                isOpen
            );

        });


        /* Fecha o menu ao clicar em um link */

        const mobileLinks =
            mobileNavigation.querySelectorAll("a");


        mobileLinks.forEach((link) => {

            link.addEventListener("click", () => {

                mobileNavigation.classList.remove("open");

                mobileButton.setAttribute(
                    "aria-expanded",
                    "false"
                );

            });

        });

    }


    /* =====================================================
       ESC FECHA MENU
    ====================================================== */

    document.addEventListener("keydown", (event) => {

        if (event.key === "Escape") {

            if (mobileNavigation) {

                mobileNavigation.classList.remove("open");

            }

            if (mobileButton) {

                mobileButton.setAttribute(
                    "aria-expanded",
                    "false"
                );

            }

        }

    });

});