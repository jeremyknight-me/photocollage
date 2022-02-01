window.jk = {
    addPhoto: function (settings) {
        let wrapper = document.getElementById('collage-wrapper');
        let frame = document.createElement('div');
        frame.id = `photo-${settings.id}`;
        frame.classList.add('photo-frame');
        if (settings.hasBorder) {
            frame.classList.add('bordered');
        }

        frame.style.zIndex = `${settings.index}`;
        const positionTop = this.getRandomIntFromZeroToMax(100);
        const positionLeft = this.getRandomIntFromZeroToMax(100);
        const half = settings.maximumSize / 2;
        frame.style.left = `calc(${positionLeft}vw - ${half}px)`;
        frame.style.top = `calc(${positionTop}vh - ${half}px)`;
        const rotation = this.getRandomIntFromAbsoluteValue(settings.maximumRotation);
        frame.style.transform = `rotate(${rotation}deg)`;
        
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

        frame.addEventListener('webkitAnimationEnd', this.handleRemovePhotoAnimationEnd);
        frame.addEventListener('animationend', this.handleRemovePhotoAnimationEnd);

        frame.appendChild(photo);
        wrapper.appendChild(frame);
    },
    removePhoto: function (id) {
        const elementId = 'photo-' + id;
        let element = document.getElementById(elementId);
        element.classList.add("removed");
    },
    getRandomIntFromAbsoluteValue: function (abs) {
        const min = -1 * abs;
        const max = abs
        return Math.floor(Math.random() * (max - min + 1)) + min;
    },
    getRandomIntFromZeroToMax: function (max) {
        const maximum = max + 1;
        return Math.floor(Math.random() * maximum);
    },
    handleRemovePhotoAnimationEnd: function (e) {
        e.target.remove();
    }
}
