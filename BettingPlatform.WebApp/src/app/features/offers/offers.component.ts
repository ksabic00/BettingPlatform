import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  OffersService,
  OfferOutcomeDto,
  OfferCategory,
  MatchOfferDto,
  MarketOfferDto,
  OfferDto
} from '../../api';
import { BetslipService } from '../../core/state/betslip.service';

type NormalizedMarket = Omit<MarketOfferDto, 'outcomes'> & { outcomes: OfferOutcomeDto[] };
type NormalizedMatch  = Omit<MatchOfferDto,  'markets'>  & { markets: NormalizedMarket[] };

type MatchVm = NormalizedMatch & {
  anyTop: boolean;
  suspended: boolean; 
};

const OUTCOME_ORDER = new Map<string, number>([
  ['1', 0], ['X', 1], ['2', 2], ['1X', 3], ['X2', 4], ['12', 5]
]);
const outcomeRank = (c?: string) => OUTCOME_ORDER.get(c ?? '') ?? 999;

@Component({
  standalone: true,
  selector: 'app-offers',
  imports: [CommonModule],
  templateUrl: './offers.component.html',
  styleUrls: ['./offers.component.scss']
})
export class OffersComponent implements OnInit {
  topMatches: MatchVm[] = [];
  regularMatches: MatchVm[] = [];

  OfferCategory = OfferCategory;

  private selectedMap = new Map<string, string>();

  constructor(private api: OffersService, private slip: BetslipService) {
    this.slip.selections$.subscribe(list => {
      this.selectedMap.clear();
      list.forEach(s => this.selectedMap.set(s.matchId, `${s.offerId}|${s.outcomeTemplateId}`));
    });
  }

  ngOnInit(): void {
    this.api.apiOffersActiveGroupedGet().subscribe(list => {
      const normalized: NormalizedMatch[] = (list ?? []).map(m => ({
        ...m,
        markets: (m.markets ?? []).map(mk => ({
          ...mk,
          outcomes: (mk.outcomes ?? [])
            .map(oc => ({ ...oc, isEnabled: (oc as any).isEnabled ?? true }))
            .sort((a, b) => outcomeRank(a.outcomeCode!) - outcomeRank(b.outcomeCode!))
        }))
      }));

      normalized.forEach(m =>
        m.markets.sort((a, b) => {
          const ra = a.marketCode === '1X2' ? 0 : 1;
          const rb = b.marketCode === '1X2' ? 0 : 1;
          return ra - rb || (a.marketCode ?? '').localeCompare(b.marketCode ?? '');
        })
      );

      const suspendedFor = (mkts: NormalizedMarket[]) => {
        const hasAnyOutcome = mkts.some(mk => mk.outcomes.length > 0);
        return hasAnyOutcome && mkts.every(mk => mk.outcomes.every(oc => oc.isEnabled === false));
      };

      const topCards: MatchVm[] = [];
      const regCards: MatchVm[] = [];

      for (const m of normalized) {
        const mkTop = m.markets.filter(x => x.category === OfferCategory.Top);
        const mkReg = m.markets.filter(x => x.category === OfferCategory.Regular);

        if (mkTop.length) {
          topCards.push({
            ...m,
            markets: mkTop,
            anyTop: true,
            suspended: suspendedFor(mkTop)
          });
        }
        if (mkReg.length) {
          regCards.push({
            ...m,
            markets: mkReg,
            anyTop: false,
            suspended: suspendedFor(mkReg)
          });
        }
      }

      this.topMatches = topCards.sort((a, b) =>
        new Date(a.startsAtUtc!).getTime() - new Date(b.startsAtUtc!).getTime()
      );
      this.regularMatches = regCards.sort((a, b) =>
        new Date(a.startsAtUtc!).getTime() - new Date(b.startsAtUtc!).getTime()
      );
    });
  }

  trackByMatch = (_: number, m: MatchVm) => m.matchId!;
  trackByMarket = (_: number, mk: NormalizedMarket) => mk.offerId!;

  isSelected(matchId?: string, offerId?: string, outcomeId?: string) {
    if (!matchId || !offerId || !outcomeId) return false;
    return this.selectedMap.get(matchId) === `${offerId}|${outcomeId}`;
  }

  pick(m: MatchVm, mk: NormalizedMarket, oc: OfferOutcomeDto) {
    if (!oc.isEnabled || m.suspended) return;

    const offer: OfferDto = {
      offerId: mk.offerId,
      matchId: m.matchId,
      homeTeam: m.homeTeam,
      awayTeam: m.awayTeam,
      startsAtUtc: m.startsAtUtc,
      marketCode: mk.marketCode,
      category: mk.category,
      outcomes: mk.outcomes
    };
    this.slip.addOrReplace(offer, oc);
  }

  private readonly MARKET_LABELS: Record<string, string> = {
    '1X2': 'Basic',
    'DoubleChance': 'Double Chance'
  };
  marketLabel(code: string | null | undefined): string {
    const c = (code ?? '').trim();
    if (!c) return '';
    return this.MARKET_LABELS[c] ?? c;
  }
}
