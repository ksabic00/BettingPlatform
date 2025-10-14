import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlayersService, CreatePlayerCommand, PlayerSummaryDto } from '../../api';
import { PlayerContextService, CurrentPlayer } from '../../core/state/player-context.service';

@Component({
  standalone: true,
  selector: 'app-players',
  imports: [CommonModule, FormsModule],
  templateUrl: './player.component.html',
})
export class PlayerComponent implements OnInit {
  players: PlayerSummaryDto[] = [];
  current: CurrentPlayer = null;

  selectedId: string | null = null; 
  name = '';                        
  msg = '';

  constructor(private api: PlayersService, private ctx: PlayerContextService) {}

  ngOnInit(): void {
    this.ctx.current$.subscribe(p => {
      this.current = p;
      if (!this.selectedId && p) this.selectedId = p.id;
    });

    this.refresh();
  }

  refresh() {
    this.api.apiPlayersGet().subscribe(list => {
      this.players = list ?? [];
      if (!this.current && this.players.length === 1) {
        this.selectedId = this.players[0].id!;
      }
    });
  }

  applySelection() {
    const p = this.players.find(x => x.id === this.selectedId!);
    if (!p) return;
    this.ctx.set({ id: p.id!, displayName: p.displayName! });
    this.msg = `Selected: ${p.displayName}`;
  }

  create() {
    const cmd: CreatePlayerCommand = { displayName: this.name.trim() };
    if (!cmd.displayName) return;

    this.api.apiPlayersPost(cmd).subscribe(id => {
      this.name = '';
      this.msg = 'Player created.';
      this.refresh();

      this.ctx.set({ id: id!, displayName: cmd.displayName! });
      this.selectedId = id!;
    });
  }
}
