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


    /* =====================================================
       CARROSSEL DE CLIENTES
    ====================================================== */

    const clientsCarousel = document.querySelector(".clients-carousel");
    const clientsTrack = document.querySelector(".clients-logos");

    if (clientsCarousel && clientsTrack) {

        const originalCards = Array.from(clientsTrack.children);
        const trackGap = parseFloat(getComputedStyle(clientsTrack).gap) || 0;
        let originalWidth = clientsTrack.scrollWidth + trackGap;

        originalCards.forEach((card) => {
            const clone = card.cloneNode(true);
            clone.setAttribute("aria-hidden", "true");
            clientsTrack.appendChild(clone);
        });

        const updateClientsWidth = () => {
            originalWidth = originalCards.reduce(
                (width, card) => width + card.getBoundingClientRect().width,
                trackGap * originalCards.length
            );
        };

        window.addEventListener("resize", updateClientsWidth, { passive: true });

        let previousTime = 0;
        let isPaused = false;
        let isDragging = false;
        let dragStartX = 0;
        let dragStartScroll = 0;
        let resumeTimer;
        const pixelsPerSecond = 34;

        clientsCarousel.addEventListener("focusin", () => isPaused = true);
        clientsCarousel.addEventListener("focusout", () => isPaused = false);

        const pauseTemporarily = () => {
            isPaused = true;
            window.clearTimeout(resumeTimer);
            resumeTimer = window.setTimeout(() => isPaused = false, 1800);
        };

        clientsCarousel.addEventListener("pointerdown", (event) => {
            if (event.pointerType === "touch") return;

            isDragging = true;
            isPaused = true;
            dragStartX = event.clientX;
            dragStartScroll = clientsCarousel.scrollLeft;
            clientsCarousel.classList.add("is-dragging");
            clientsCarousel.setPointerCapture(event.pointerId);
        });

        clientsCarousel.addEventListener("pointermove", (event) => {
            if (!isDragging) return;
            clientsCarousel.scrollLeft = dragStartScroll - (event.clientX - dragStartX);
        });

        const finishDragging = (event) => {
            if (!isDragging) return;

            isDragging = false;
            clientsCarousel.classList.remove("is-dragging");

            if (clientsCarousel.hasPointerCapture(event.pointerId)) {
                clientsCarousel.releasePointerCapture(event.pointerId);
            }

            pauseTemporarily();
        };

        clientsCarousel.addEventListener("pointerup", finishDragging);
        clientsCarousel.addEventListener("pointercancel", finishDragging);
        clientsCarousel.addEventListener("touchstart", pauseTemporarily, { passive: true });
        clientsCarousel.addEventListener("touchend", pauseTemporarily, { passive: true });
        clientsCarousel.addEventListener("wheel", pauseTemporarily, { passive: true });

        const animateClients = (time) => {
            if (previousTime && !isPaused) {
                clientsCarousel.scrollLeft += ((time - previousTime) / 1000) * pixelsPerSecond;

                if (clientsCarousel.scrollLeft >= originalWidth) {
                    clientsCarousel.scrollLeft -= originalWidth;
                }
            }

            previousTime = time;
            window.requestAnimationFrame(animateClients);
        };

        window.requestAnimationFrame(animateClients);
    }

});
