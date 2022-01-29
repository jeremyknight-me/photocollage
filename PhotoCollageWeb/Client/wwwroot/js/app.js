window.jk = {
    addPhoto: function (settings) {
        let wrapper = document.getElementById('collage-wrapper');
        let frame = document.createElement('div');
        frame.id = `photo-${settings.id}`;
        frame.classList.add('photo-frame');
        if (settings.hasBorder) {
            frame.classList.add('bordered');
        }

        const positionTop = Math.floor(Math.random() * 101);
        const positionLeft = Math.floor(Math.random() * 101);
        const min = -1 * settings.maximumRotation;
        const max = settings.maximumRotation
        const rotation = Math.floor(Math.random() * (max - min + 1)) + min;
        const half = settings.maximumSize / 2;
        frame.style.left = `calc(${positionLeft}vw - ${half}px)`;
        frame.style.top = `calc(${positionTop}vh - ${half}px)`;
        frame.style.transform = `rotate(${rotation}deg)`;
        frame.style.zIndex = `${settings.index}`;

        let photo = document.createElement('img');
        photo.src = settings.source;
        photo.style.maxHeight = `${settings.maximumSize}px`;
        photo.style.maxWidth = `${settings.maximumSize}px`;
        if (settings.isGrayscale) {
            photo.style.filter = 'grayscale(1)';
        }

        let loading = document.getElementById('loading');
        if (loading) {
            loading.remove();
        }

        frame.appendChild(photo);
        wrapper.appendChild(frame);
    },
    removePhoto: function (id) {
        const elementId = 'photo-' + id;
        let element = document.getElementById(elementId);
        element.classList.add("removed");
    }
}
