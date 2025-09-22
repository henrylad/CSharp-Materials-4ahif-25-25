document.addEventListener('DOMContentLoaded', function () {
   const image = document.getElementById('spinningImage');

   if (image) {
      let angle = 0;
      const rotationSpeed = 5; 
      let posX = 0;
      let posY = 0;
      let moveRight = true;
      let scale = 1;
      let growing = true;

      let maxX = window.innerWidth - 100; 

      function animateImage() {

         angle += rotationSpeed;
         if (angle >= 360) {
            angle = 0;
         }
         
         if (moveRight) {
            posX += 5;
            if (posX > maxX) moveRight = false;
         } else {
            posX -= 5;
            if (posX < 0) moveRight = true;
         }
         
         if (growing) {
            scale += 0.005;
            if (scale > 3.0) growing = false;
         } else {
            scale -= 0.005;
            if (scale < 0.5) growing = true;
         }

         image.style.transform = `translate(${posX}px, ${posY}px) rotate(${angle}deg) scale(${scale})`;
         
         requestAnimationFrame(animateImage);
      }

      image.style.position = 'absolute';
      image.style.transformOrigin = 'center center';
      
      image.addEventListener('load', function () {
         requestAnimationFrame(animateImage);
      });
      if (image.complete) {
         requestAnimationFrame(animateImage);
      }
      
      window.addEventListener('resize', function() {
         maxX = window.innerWidth - 100;
      });
   }

   // Hover / focus handlers for background swap
   function setBodyBg(name){ document.body.classList.add('bg-' + name); }
   function clearBodyBg(name){ document.body.classList.remove('bg-' + name); }

   document.querySelectorAll('.hover-trigger').forEach(function(el){
      const bg = el.getAttribute('data-bg');
      if (!bg) return;

      el.addEventListener('mouseenter', function(){ setBodyBg(bg); });
      el.addEventListener('mouseleave', function(){ clearBodyBg(bg); });
      el.addEventListener('focus', function(){ setBodyBg(bg); });
      el.addEventListener('blur', function(){ clearBodyBg(bg); });

      // touch support: toggle on touchstart briefly
      el.addEventListener('touchstart', function touchHandler(e){
         setBodyBg(bg);
         setTimeout(function(){ clearBodyBg(bg); }, 1200);
      }, {passive: true});
   });

   // YouTube-on-hover: creates a muted autoplay iframe in #yt-silent-container
   (function(){
      const container = document.getElementById('yt-silent-container');
      const unmuteBtn = document.getElementById('yt-unmute');
      let currentVid = null;
      let iframeEl = null;

      function extractVideoId(input){
         if (!input) return null;
         // if looks like a full URL
         try{
            const u = new URL(input);
            if (u.hostname.includes('youtube')){
               return u.searchParams.get('v') || null;
            }
         }catch(e){ /* not a URL */ }
         // otherwise assume it's an ID
         return input;
      }

   function createIframe(videoId){
         if (!videoId) return null;
      // autoplay muted; playsinline helps on mobile. Controls=1 so user can pause.
      const src = `https://www.youtube.com/embed/${videoId}?autoplay=1&mute=1&controls=1&rel=0&playsinline=1&modestbranding=1&enablejsapi=1`;
      const ifr = document.createElement('iframe');
      ifr.src = src;
      ifr.setAttribute('allow', 'autoplay; encrypted-media; picture-in-picture; fullscreen');
      ifr.setAttribute('allowfullscreen','');
      ifr.style.width = '100%';
      ifr.style.height = '100%';
      ifr.style.border = '0';
      return ifr;
      }

      function showUnmute(){
         unmuteBtn.style.display = 'inline-block';
         unmuteBtn.setAttribute('aria-hidden','false');
      }
      function hideUnmute(){
         unmuteBtn.style.display = 'none';
         unmuteBtn.setAttribute('aria-hidden','true');
      }

      // Click-to-toggle behavior for yt-trigger buttons
      document.querySelectorAll('.yt-trigger').forEach(function(el){
         const raw = el.getAttribute('data-video');
         const vid = extractVideoId(raw);
         if (!vid) return;

         el.addEventListener('click', function(){
            if (currentVid === vid && iframeEl){
               // close
               if (iframeEl && container.contains(iframeEl)) container.removeChild(iframeEl);
               iframeEl = null;
               currentVid = null;
               container.style.display = 'none';
               document.body.style.overflow = '';
               hideUnmute();
               return;
            }

            // open
            if (iframeEl && container.contains(iframeEl)) container.removeChild(iframeEl);
            iframeEl = createIframe(vid);
            if (iframeEl) {
               container.appendChild(iframeEl);
               container.style.display = 'block';
               document.body.style.overflow = 'hidden';
            }
            currentVid = vid;
            setTimeout(showUnmute, 800);
         });
      });

      unmuteBtn.addEventListener('click', function(){
         if (!iframeEl) return;
         // replace iframe with one that has sound allowed (some browsers may still block)
         const vid = currentVid;
         if (!vid) return;
         if (iframeEl && container.contains(iframeEl)) container.removeChild(iframeEl);
         const src = `https://www.youtube.com/embed/${vid}?autoplay=1&mute=0&controls=1&rel=0&playsinline=1&modestbranding=1`;
         const ifr = document.createElement('iframe');
         ifr.src = src;
         ifr.width = '0';
         ifr.height = '0';
         ifr.style.border = '0';
         ifr.style.opacity = '0';
         ifr.setAttribute('allow', 'autoplay; encrypted-media');
         container.appendChild(ifr);
         iframeEl = ifr;
         hideUnmute();
      });

         // close on ESC
         document.addEventListener('keydown', function(e){
            if (e.key === 'Escape' && iframeEl) {
               // trigger leave behavior
               document.querySelectorAll('.yt-trigger').forEach(function(el){ el.dispatchEvent(new Event('mouseleave')); });
            }
         });

         // clicking outside iframe closes it
         container.addEventListener('click', function(e){
            if (e.target === container && iframeEl){
               document.querySelectorAll('.yt-trigger').forEach(function(el){ el.dispatchEvent(new Event('mouseleave')); });
            }
         });
   })();
});