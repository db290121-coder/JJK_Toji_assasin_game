// Game Configuration and Characters
const CHARACTERS = {
    yuji: { name: 'Yuji Itadori', hp: 80, damage: 15, speed: 3, color: '#FF6B6B' },
    megumi: { name: 'Megumi Fushiguro', hp: 100, damage: 20, speed: 3.5, color: '#4ECDC4' },
    nobara: { name: 'Nobara Kugisaki', hp: 75, damage: 18, speed: 3.2, color: '#FFE66D' },
    gojo: { name: 'Satoru Gojo', hp: 150, damage: 30, speed: 4, color: '#95E1D3' },
    sukuna: { name: 'Ryomen Sukuna', hp: 180, damage: 35, speed: 3.8, color: '#FF6348' },
    jogo: { name: 'Jogo', hp: 110, damage: 25, speed: 3.3, color: '#FF8C42' },
    mahito: { name: 'Mahito', hp: 95, damage: 22, speed: 3.6, color: '#9D4EDD' }
};

const STAGES = [
    {
        name: 'Tokyo Jujutsu High',
        targets: ['yuji', 'nobara'],
        description: 'Eliminate low-level sorcerers at the academy'
    },
    {
        name: 'Hidden Shrine',
        targets: ['megumi', 'jogo'],
        description: 'Face stronger opponents in an isolated location'
    },
    {
        name: 'Cursed Grounds',
        targets: ['mahito', 'gojo'],
        description: 'Navigate through cursed territory'
    },
    {
        name: 'Final Confrontation',
        targets: ['sukuna'],
        description: 'Face the King of Curses himself'
    }
];

// Main Game Class
class Toji_Game {
    constructor() {
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');
        this.gameRunning = false;
        this.gamePaused = false;
        this.currentStage = 0;
        this.score = 0;
        this.targetsEliminated = 0;
        
        // Player
        this.player = {
            x: 100,
            y: 300,
            width: 40,
            height: 50,
            hp: 100,
            maxHp: 100,
            stamina: 100,
            maxStamina: 100,
            speed: 5,
            color: '#2C3E50',
            velocityY: 0,
            velocityX: 0
        };

        // Game entities
        this.enemies = [];
        this.projectiles = [];
        this.effects = [];
        this.items = [];

        // Input handling
        this.keys = {};
        this.mousePos = { x: 0, y: 0 };

        // Setup event listeners
        this.setupEventListeners();
    }

    setupEventListeners() {
        document.addEventListener('keydown', (e) => {
            this.keys[e.key.toLowerCase()] = true;
            if (e.key === ' ') this.playerAttack();
            if (e.key === 'e' || e.key === 'E') this.pickupItem();
            if (e.key === 'q' || e.key === 'Q') this.specialAbility();
            if (e.key === 'Escape') this.togglePause();
        });

        document.addEventListener('keyup', (e) => {
            this.keys[e.key.toLowerCase()] = false;
        });

        document.addEventListener('mousemove', (e) => {
            const rect = this.canvas.getBoundingClientRect();
            this.mousePos.x = e.clientX - rect.left;
            this.mousePos.y = e.clientY - rect.top;
        });
    }

    startGame() {
        document.getElementById('mainMenu').classList.add('hidden');
        document.getElementById('gameScreen').classList.remove('hidden');
        this.gameRunning = true;
        this.gamePaused = false;
        this.currentStage = 0;
        this.score = 0;
        this.targetsEliminated = 0;
        this.loadStage();
        this.gameLoop();
    }

    loadStage() {
        this.enemies = [];
        this.projectiles = [];
        this.effects = [];
        this.items = [];
        this.player.hp = this.player.maxHp;
        this.player.stamina = this.player.maxStamina;

        const stage = STAGES[this.currentStage];
        const targetCount = stage.targets.length;

        // Spawn enemies
        stage.targets.forEach((charKey, index) => {
            const char = CHARACTERS[charKey];
            const enemy = {
                ...char,
                x: 700 - (index * 150),
                y: Math.random() * (this.canvas.height - 100) + 50,
                velocityX: -2,
                velocityY: 0,
                targetId: charKey,
                originalHp: char.hp,
                attackCooldown: 0,
                detectionRange: 300
            };
            this.enemies.push(enemy);
        });

        this.updateInfo(`Stage ${this.currentStage + 1}: ${stage.name}\n${stage.description}\nTargets: ${targetCount}`);
    }

    gameLoop() {
        this.update();
        this.draw();
        requestAnimationFrame(() => this.gameLoop());
    }

    update() {
        if (this.gamePaused || !this.gameRunning) return;

        // Player movement
        this.player.velocityX = 0;
        if (this.keys['arrowleft'] || this.keys['a']) this.player.velocityX = -this.player.speed;
        if (this.keys['arrowright'] || this.keys['d']) this.player.velocityX = this.player.speed;
        if (this.keys['arrowup'] || this.keys['w']) this.player.y = Math.max(0, this.player.y - this.player.speed);
        if (this.keys['arrowdown'] || this.keys['s']) this.player.y = Math.min(this.canvas.height - this.player.height, this.player.y + this.player.speed);

        // Update player position
        this.player.x += this.player.velocityX;
        this.player.x = Math.max(0, Math.min(this.canvas.width - this.player.width, this.player.x));

        // Regenerate stamina
        this.player.stamina = Math.min(this.player.maxStamina, this.player.stamina + 0.5);

        // Update enemies
        this.enemies.forEach((enemy, index) => {
            enemy.x += enemy.velocityX;
            
            // Enemy behavior - chase if close enough
            const distToPlayer = Math.hypot(this.player.x - enemy.x, this.player.y - enemy.y);
            
            if (distToPlayer < enemy.detectionRange) {
                // Chase player
                const angle = Math.atan2(this.player.y - enemy.y, this.player.x - enemy.x);
                enemy.velocityX = Math.cos(angle) * 1.5;
                enemy.velocityY = Math.sin(angle) * 1.5;
            } else {
                enemy.velocityY = 0;
            }

            enemy.y += enemy.velocityY;
            enemy.y = Math.max(0, Math.min(this.canvas.height - 50, enemy.y));

            // Enemy attack
            if (distToPlayer < 80) {
                enemy.attackCooldown--;
                if (enemy.attackCooldown <= 0) {
                    this.createProjectile(enemy.x, enemy.y, this.player.x, this.player.y, 'enemy');
                    enemy.attackCooldown = 60;
                }
            } else {
                enemy.attackCooldown = 0;
            }
        });

        // Update projectiles
        this.projectiles = this.projectiles.filter(proj => {
            proj.x += proj.velocityX;
            proj.y += proj.velocityY;

            // Check collision with player
            if (proj.type === 'enemy' && this.checkCollision(proj, this.player)) {
                this.player.hp -= 10;
                this.createExplosion(proj.x, proj.y, 'hit');
                return false;
            }

            // Check collision with enemies
            this.enemies.forEach((enemy, index) => {
                if (proj.type === 'player' && this.checkCollision(proj, enemy)) {
                    enemy.hp -= proj.damage;
                    this.createExplosion(proj.x, proj.y, 'hit');
                    if (enemy.hp <= 0) {
                        this.enemies.splice(index, 1);
                        this.score += 500;
                        this.targetsEliminated++;
                        this.spawnItem(enemy.x, enemy.y);
                    }
                    return false;
                }
            });

            return proj.x > -50 && proj.x < this.canvas.width + 50 && proj.y > -50 && proj.y < this.canvas.height + 50;
        });

        // Update effects
        this.effects = this.effects.filter(effect => {
            effect.life--;
            return effect.life > 0;
        });

        // Update items
        this.items.forEach(item => {
            item.y += 2;
        });

        // Check item pickup
        this.items = this.items.filter(item => {
            if (this.checkCollision(item, this.player)) {
                this.applyItem(item);
                return false;
            }
            return item.y < this.canvas.height;
        });

        // Check stage completion
        if (this.enemies.length === 0 && this.currentStage < STAGES.length) {
            this.currentStage++;
            if (this.currentStage >= STAGES.length) {
                this.endGame(true);
            } else {
                this.loadStage();
            }
        }

        // Check game over
        if (this.player.hp <= 0) {
            this.endGame(false);
        }

        // Update UI
        this.updateUI();
    }

    draw() {
        // Clear canvas
        this.ctx.fillStyle = 'rgba(135, 206, 235, 0.1)';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

        // Draw player
        this.drawPlayer();

        // Draw enemies
        this.enemies.forEach(enemy => this.drawEnemy(enemy));

        // Draw projectiles
        this.projectiles.forEach(proj => this.drawProjectile(proj));

        // Draw items
        this.items.forEach(item => this.drawItem(item));

        // Draw effects
        this.effects.forEach(effect => this.drawEffect(effect));

        // Draw info
        this.drawInfo();
    }

    drawPlayer() {
        // Body
        this.ctx.fillStyle = this.player.color;
        this.ctx.fillRect(this.player.x, this.player.y, this.player.width, this.player.height);

        // Head
        this.ctx.beginPath();
        this.ctx.arc(this.player.x + this.player.width / 2, this.player.y - 10, 12, 0, Math.PI * 2);
        this.ctx.fill();

        // HP bar above player
        const barWidth = this.player.width;
        const hpPercent = this.player.hp / this.player.maxHp;
        this.ctx.fillStyle = '#333';
        this.ctx.fillRect(this.player.x, this.player.y - 30, barWidth, 8);
        this.ctx.fillStyle = hpPercent > 0.5 ? '#4caf50' : '#ff9800';
        this.ctx.fillRect(this.player.x, this.player.y - 30, barWidth * hpPercent, 8);
        this.ctx.strokeStyle = '#fff';
        this.ctx.lineWidth = 1;
        this.ctx.strokeRect(this.player.x, this.player.y - 30, barWidth, 8);

        // Name
        this.ctx.fillStyle = '#fff';
        this.ctx.font = 'bold 12px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText('TOJI', this.player.x + this.player.width / 2, this.player.y - 40);
    }

    drawEnemy(enemy) {
        // Body
        this.ctx.fillStyle = enemy.color;
        this.ctx.fillRect(enemy.x, enemy.y, 35, 45);

        // Head
        this.ctx.beginPath();
        this.ctx.arc(enemy.x + 17.5, enemy.y - 10, 10, 0, Math.PI * 2);
        this.ctx.fill();

        // HP bar
        const barWidth = 35;
        const hpPercent = enemy.hp / enemy.originalHp;
        this.ctx.fillStyle = '#333';
        this.ctx.fillRect(enemy.x, enemy.y - 25, barWidth, 6);
        this.ctx.fillStyle = hpPercent > 0.5 ? '#4caf50' : '#ff9800';
        this.ctx.fillRect(enemy.x, enemy.y - 25, barWidth * hpPercent, 6);
        this.ctx.strokeStyle = '#fff';
        this.ctx.lineWidth = 1;
        this.ctx.strokeRect(enemy.x, enemy.y - 25, barWidth, 6);

        // Name
        this.ctx.fillStyle = '#fff';
        this.ctx.font = 'bold 10px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText(enemy.name.split(' ')[0], enemy.x + 17.5, enemy.y - 35);
    }

    drawProjectile(proj) {
        this.ctx.fillStyle = proj.color;
        this.ctx.beginPath();
        this.ctx.arc(proj.x, proj.y, proj.radius, 0, Math.PI * 2);
        this.ctx.fill();
        
        // Projectile trail
        this.ctx.strokeStyle = proj.color;
        this.ctx.lineWidth = 2;
        this.ctx.globalAlpha = 0.5;
        this.ctx.beginPath();
        this.ctx.arc(proj.x - proj.velocityX * 5, proj.y - proj.velocityY * 5, proj.radius, 0, Math.PI * 2);
        this.ctx.stroke();
        this.ctx.globalAlpha = 1;
    }

    drawItem(item) {
        this.ctx.fillStyle = item.color;
        this.ctx.fillRect(item.x, item.y, 20, 20);
        this.ctx.strokeStyle = '#FFD700';
        this.ctx.lineWidth = 2;
        this.ctx.strokeRect(item.x, item.y, 20, 20);
    }

    drawEffect(effect) {
        this.ctx.fillStyle = effect.color;
        this.ctx.globalAlpha = effect.life / effect.maxLife;
        this.ctx.beginPath();
        this.ctx.arc(effect.x, effect.y, effect.radius, 0, Math.PI * 2);
        this.ctx.fill();
        this.ctx.globalAlpha = 1;
    }

    drawInfo() {
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
        this.ctx.fillRect(10, 10, 300, 60);
        this.ctx.fillStyle = '#e94560';
        this.ctx.font = 'bold 12px Arial';
        this.ctx.textAlign = 'left';
        this.ctx.fillText(`Stage: ${this.currentStage + 1}/${STAGES.length}`, 20, 30);
        this.ctx.fillText(`Score: ${this.score}`, 20, 50);
    }

    createProjectile(fromX, fromY, toX, toY, type) {
        const angle = Math.atan2(toY - fromY, toX - fromX);
        const speed = 4;

        this.projectiles.push({
            x: fromX,
            y: fromY,
            velocityX: Math.cos(angle) * speed,
            velocityY: Math.sin(angle) * speed,
            radius: type === 'player' ? 8 : 6,
            type: type,
            color: type === 'player' ? '#FFD700' : '#FF6B6B',
            damage: 20
        });
    }

    createExplosion(x, y, type) {
        for (let i = 0; i < 8; i++) {
            this.effects.push({
                x: x,
                y: y,
                radius: Math.random() * 15 + 5,
                color: type === 'hit' ? '#FF6B6B' : '#FFD700',
                life: 20,
                maxLife: 20
            });
        }
    }

    spawnItem(x, y) {
        const items = [
            { type: 'health', color: '#4CAF50', effect: 'heal' },
            { type: 'stamina', color: '#2196F3', effect: 'stamina' },
            { type: 'power', color: '#FFD700', effect: 'damage' }
        ];
        
        const item = {
            x: x,
            y: y,
            ...items[Math.floor(Math.random() * items.length)]
        };
        this.items.push(item);
    }

    applyItem(item) {
        switch (item.effect) {
            case 'heal':
                this.player.hp = Math.min(this.player.maxHp, this.player.hp + 30);
                this.score += 100;
                break;
            case 'stamina':
                this.player.stamina = Math.min(this.player.maxStamina, this.player.stamina + 40);
                this.score += 50;
                break;
            case 'damage':
                setTimeout(() => {
                    const projectileDamage = 20;
                }, 5000);
                this.score += 150;
                break;
        }
    }

    playerAttack() {
        if (this.player.stamina < 15) return;

        this.player.stamina -= 15;
        const angle = Math.atan2(this.mousePos.y - this.player.y, this.mousePos.x - this.player.x);

        this.createProjectile(
            this.player.x + this.player.width / 2,
            this.player.y + this.player.height / 2,
            this.mousePos.x,
            this.mousePos.y,
            'player'
        );

        this.createExplosion(this.player.x + this.player.width / 2, this.player.y + this.player.height / 2, 'attack');
    }

    specialAbility() {
        if (this.player.stamina < 50) return;

        this.player.stamina -= 50;
        
        // Create multiple projectiles
        for (let i = 0; i < 8; i++) {
            const angle = (Math.PI * 2 * i) / 8;
            const speed = 5;
            this.projectiles.push({
                x: this.player.x + this.player.width / 2,
                y: this.player.y + this.player.height / 2,
                velocityX: Math.cos(angle) * speed,
                velocityY: Math.sin(angle) * speed,
                radius: 6,
                type: 'player',
                color: '#FFD700',
                damage: 25
            });
        }
    }

    pickupItem() {
        // Items auto-pickup when touched
    }

    checkCollision(obj1, obj2) {
        const rect1 = {
            x: obj1.x,
            y: obj1.y,
            width: obj1.width || obj1.radius * 2,
            height: obj1.height || obj1.radius * 2
        };

        const rect2 = {
            x: obj2.x,
            y: obj2.y,
            width: obj2.width || 20,
            height: obj2.height || 20
        };

        return rect1.x < rect2.x + rect2.width &&
               rect1.x + rect1.width > rect2.x &&
               rect1.y < rect2.y + rect2.height &&
               rect1.y + rect1.height > rect2.y;
    }

    updateUI() {
        document.getElementById('hpFill').style.width = (this.player.hp / this.player.maxHp) * 100 + '%';
        document.getElementById('hpText').textContent = `${Math.ceil(this.player.hp)}/${this.player.maxHp}`;

        document.getElementById('staminaFill').style.width = (this.player.stamina / this.player.maxStamina) * 100 + '%';
        document.getElementById('staminaText').textContent = `${Math.ceil(this.player.stamina)}/${this.player.maxStamina}`;

        document.getElementById('missionText').textContent = STAGES[this.currentStage]?.name || 'Complete';
        document.getElementById('targetText').textContent = this.targetsEliminated;
    }

    updateInfo(text) {
        document.getElementById('infoBox').textContent = text;
    }

    endGame(victory) {
        this.gameRunning = false;
        document.getElementById('gameScreen').classList.add('hidden');
        document.getElementById('gameOverScreen').classList.remove('hidden');

        document.getElementById('gameOverTitle').textContent = victory ? 'MISSION COMPLETE!' : 'MISSION FAILED';
        document.getElementById('finalTargets').textContent = this.targetsEliminated;
        document.getElementById('finalScore').textContent = this.score;
    }

    togglePause() {
        if (!this.gameRunning) return;
        this.gamePaused = !this.gamePaused;
        
        if (this.gamePaused) {
            document.getElementById('pauseScreen').classList.remove('hidden');
        } else {
            document.getElementById('pauseScreen').classList.add('hidden');
        }
    }

    showMenu() {
        this.gameRunning = false;
        this.gamePaused = false;
        document.getElementById('mainMenu').classList.remove('hidden');
        document.getElementById('gameScreen').classList.add('hidden');
        document.getElementById('gameOverScreen').classList.add('hidden');
        document.getElementById('pauseScreen').classList.add('hidden');
        document.getElementById('storyScreen').classList.add('hidden');
        document.getElementById('controlsScreen').classList.add('hidden');
    }

    showStory() {
        document.getElementById('mainMenu').classList.add('hidden');
        document.getElementById('storyScreen').classList.remove('hidden');
    }

    showControls() {
        document.getElementById('mainMenu').classList.add('hidden');
        document.getElementById('controlsScreen').classList.remove('hidden');
    }

    returnToMenu() {
        this.togglePause();
        this.showMenu();
    }
}

// Initialize game
const game = new Toji_Game();
