namespace GameplayLearning.EventSystem
{
    public static class EventDefine
    {
        // 游戏状态事件
        public const string GameStarted = "game_started";
        public const string GamePaused = "game_paused";
        public const string GameResumed = "game_resumed";
        public const string GameOver = "game_over";
        
        // 玩家相关事件
        public const string PlayerSpawned = "player_spawned";
        public const string PlayerDied = "player_died";
        public const string PlayerHealthChanged = "player_health_changed";
        public const string PlayerScoreChanged = "player_score_changed";
        
        // 魔法系统事件（适用于您的SpellCaster系统）
        public const string SpellCast = "spell_cast";
        public const string SpellHit = "spell_hit";
        public const string SpellLearned = "spell_learned";
        public const string WandChanged = "wand_changed";
        
        // UI事件
        public const string MenuOpened = "menu_opened";
        public const string MenuClosed = "menu_closed";
        public const string DialogueStarted = "dialogue_started";
        public const string DialogueEnded = "dialogue_ended";
    }
}