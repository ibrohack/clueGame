using MongoDB.Driver;
using clueGame.Models.Mongo;

namespace clueGame.Services;

public class LoreSeeder
{
    private readonly IMongoDatabase _db;

    public LoreSeeder(MongoDbService mongo)
    {
        _db = mongo.Database;
    }

    public async Task SeedAsync()
    {
        await SeedCharactersAsync();
        await SeedWeaponsAsync();
        await SeedLocationsAsync();
    }

    // ── Local record types ────────────────────────────────────────────────────

    private sealed record CharacterLore(
        string Id, string Quote, string Temperament, string Location,
        string Status, string KnownAssociates, string DistinguishingMark,
        string Motive, List<string> Description);

    private sealed record WeaponLore(
        string Id, string Quote, string Category, string Lethality,
        string Origin, string Condition, string HandledBy,
        string Evidence, List<string> Description);

    private sealed record LocationLore(
        string Id, string Quote, string Floor, string SecurityLevel,
        string OccupancyStatus, string KnownOccupants, string LastIncident,
        string AccessPoints, string Notes, List<string> Description);

    // ── Characters ────────────────────────────────────────────────────────────
    // IDs from database/characters.json:
    //   character_1 = Fern Malachite
    //   character_2 = Chrysant Pyrite
    //   character_3 = Hyacinth Sapphire
    //   character_4 = Poppy Ruby
    //   character_5 = Dahlia Onix
    //   character_6 = Gardenia Quartz

    private async Task SeedCharactersAsync()
    {
        var col = _db.GetCollection<MongoCharacter>("characters");

        var loreList = new CharacterLore[]
        {
            new("character_2",  // Chrysant Pyrite
                "Gold is merely a state of mind, darling. Mine happens to be permanent.",
                "Opportunistic", "Solarium", "Prime Suspect",
                "Dahlia Onix, Poppy Ruby", "Gold-tipped fingernails",
                "CLASSIFIED BY ORDER OF THE BLACKWOOD ESTATE INVESTIGATION BOARD",
                new List<string>
                {
                    "Chrysant Pyrite arrived at the Blackwood Estate with the quiet assurance of someone who has never been told no. Her background in estate acquisition — a polite term for what many have called predatory inheritance brokering — placed her among the manor's most financially motivated guests.",
                    "Witnesses recall her spending an unusual amount of time in the Solarium on the evening in question, ostensibly admiring the orchid collection. Those who know her better suspect she was admiring the view of the safe room beyond the east garden wall.",
                    "Her gold-tipped fingernails were found to bear trace residue consistent with the study's lacquered bookcase. When questioned, she smiled and said she had 'browsed a volume or two.' The investigator noted that the bookcase in question contains the estate's financial ledgers."
                }),

            new("character_5",  // Dahlia Onix
                "I prefer things black and white — mostly black.",
                "Calculating", "Cinema", "Person of Interest",
                "Fern Malachite", "Obsidian pendant",
                "CLASSIFIED BY ORDER OF THE BLACKWOOD ESTATE INVESTIGATION BOARD",
                new List<string>
                {
                    "Dahlia Onix carries herself with the precision of someone who has spent years studying the angles of every room she enters. Her professional history in forensic architecture — the art of reconstructing events from spatial evidence — makes her both an invaluable ally and an unsettling suspect.",
                    "She was observed in the Cinema for much of the evening, claiming to have been reviewing old reels of the estate's private collection. Security footage, however, shows a fifteen-minute gap between 10:42 and 10:57 PM that she has declined to explain satisfactorily.",
                    "The obsidian pendant she wears is said to have belonged to the original estate architect. How it came to be in her possession remains a matter of some debate. When pressed, she simply adjusts it and says: 'Provenance is overrated. Possession is far more interesting.'"
                }),

            new("character_1",  // Fern Malachite
                "Nature is patient. I have learned from the best.",
                "Patient", "Garden", "Under Observation",
                "Chrysant Pyrite", "Green-stained fingers",
                "CLASSIFIED BY ORDER OF THE BLACKWOOD ESTATE INVESTIGATION BOARD",
                new List<string>
                {
                    "Fern Malachite is the estate's resident botanist — or so the invitation read. Her credentials, while impeccable on paper, were issued by an institution that closed its doors four years prior. This discrepancy was noted but not yet acted upon at the time of the incident.",
                    "She was found tending to the night-blooming jasmine near the east boundary when the commotion began. Her green-stained fingers, she explained, were the result of pruning malachite-fed ferns. A convincing answer, save for the fact that malachite-fed ferns do not exist.",
                    "Those who have spoken with her at length note an unusual quality of stillness — a person entirely unbothered by silence, as comfortable waiting as breathing. Whether this patience is botanical or predatory in nature is precisely what the investigation seeks to determine."
                }),

            new("character_6",  // Gardenia Quartz
                "Everything reflects, if you know how to look.",
                "Perceptive", "Helipad", "Person of Interest",
                "Hyacinth Sapphire", "Crystal brooch",
                "CLASSIFIED BY ORDER OF THE BLACKWOOD ESTATE INVESTIGATION BOARD",
                new List<string>
                {
                    "Gardenia Quartz arrived by private helicopter, which in itself is unremarkable among this set. What is remarkable is that no flight plan was filed, no pilot has come forward, and the helicopter in question was gone before dawn without a trace on any radar system within forty miles.",
                    "Her crystal brooch — a family heirloom by her account, a surveillance device by the suspicion of at least two investigators — catches light in ways that seem almost deliberate. She deflects questions about it with observations about the room's ambient luminosity.",
                    "She is, by all accounts, extraordinarily observant. Three separate guests reported feeling watched in her presence, despite her never appearing to look directly at them. Whether this is a social gift or professional training, the board has not yet concluded."
                }),

            new("character_3",  // Hyacinth Sapphire
                "Blue blood runs deep — and cold.",
                "Composed", "Swimming Pool", "Prime Suspect",
                "Gardenia Quartz, Poppy Ruby", "Sapphire earrings",
                "CLASSIFIED BY ORDER OF THE BLACKWOOD ESTATE INVESTIGATION BOARD",
                new List<string>
                {
                    "Hyacinth Sapphire is old money in the truest and coldest sense. Her family's connection to the Blackwood Estate predates the current owner by three generations, and she has made no secret of her belief that the estate should have remained under the Sapphire trust.",
                    "She was found near the swimming pool at approximately 11:15 PM, wearing an evening gown that most guests would not choose for poolside attendance. She stated she was 'taking air.' The chlorine residue detected on her left sleeve suggests something more aquatic than atmospheric.",
                    "Her composure throughout questioning has been described alternately as dignified and deeply unsettling. She answered every question in precisely the number of words required — no more, no less — and twice corrected the investigator's grammar. The sapphire earrings she wore match a description from a missing estate inventory manifest dated 1987."
                }),

            new("character_4",  // Poppy Ruby
                "Red is the color of both love and warning. I prefer ambiguity.",
                "Volatile", "Gym", "Under Surveillance",
                "Chrysant Pyrite, Hyacinth Sapphire", "Ruby-red lip stain",
                "CLASSIFIED BY ORDER OF THE BLACKWOOD ESTATE INVESTIGATION BOARD",
                new List<string>
                {
                    "Poppy Ruby is the kind of person who walks into a room and immediately recalibrates its center of gravity. Her career as a performance artist — specializing in what she calls 'consequence theatre' — has involved stunts that have alternately fascinated and horrified critics across three continents.",
                    "She was using the private gymnasium at the time of the incident, a claim supported by the security log but contradicted by the state of the equipment, which showed no signs of recent use. The punching bag, however, bore a fresh impression consistent with a strike pattern investigators found noteworthy.",
                    "Her ruby-red lip stain has been sampled and analyzed. It is a bespoke formulation available from exactly one supplier. That supplier's client list is the subject of an ongoing, unrelated inquiry. Poppy Ruby, when informed of this, laughed. It was not a reassuring laugh."
                }),
        };

        foreach (var lore in loreList)
        {
            var existing = await col.Find(c => c.Id == lore.Id).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(existing?.Quote)) continue;

            var filter = Builders<MongoCharacter>.Filter.Eq(c => c.Id, lore.Id);
            var update = Builders<MongoCharacter>.Update
                .Set(c => c.Quote, lore.Quote)
                .Set(c => c.Temperament, lore.Temperament)
                .Set(c => c.Location, lore.Location)
                .Set(c => c.Status, lore.Status)
                .Set(c => c.KnownAssociates, lore.KnownAssociates)
                .Set(c => c.DistinguishingMark, lore.DistinguishingMark)
                .Set(c => c.Motive, lore.Motive)
                .Set(c => c.Description, lore.Description);
            await col.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }
    }

    // ── Weapons ───────────────────────────────────────────────────────────────
    // IDs from database/weapons.json:
    //   weapon_1 = Mower
    //   weapon_2 = Dumbel
    //   weapon_3 = Flowerpot
    //   weapon_4 = Plastic Bag
    //   weapon_5 = Chain Saw
    //   weapon_6 = Rat Poison

    private async Task SeedWeaponsAsync()
    {
        var col = _db.GetCollection<MongoWeapon>("weapons");

        var loreList = new WeaponLore[]
        {
            new("weapon_5",  // Chain Saw
                "Precision is the difference between craft and carnage. I make no such distinctions.",
                "Power Tool", "Extreme", "Basement Workshop",
                "Recently Serviced", "Unknown",
                "EVIDENCE SEALED — ACCESS RESTRICTED TO LEAD INVESTIGATOR",
                new List<string>
                {
                    "A professional-grade chain saw discovered in the basement workshop, its chain freshly sharpened and its housing recently wiped clean. The model is industrial, far beyond the requirements of ordinary estate maintenance, and was not listed in the groundskeeper's equipment inventory.",
                    "Forensic analysis of the blade revealed trace biological material inconsistent with wood fiber. The lubricant applied to the chain is a specialized compound available only through licensed contractors — none of whom have been identified as guests at the estate.",
                    "The noise generated by this weapon would have been considerable. That no guest reported hearing anything unusual on the evening in question suggests either remarkable sound insulation in the workshop — or remarkable coordination in the silence above it."
                }),

            new("weapon_2",  // Dumbel
                "Weight is a matter of perspective. Applied correctly, it ends all perspectives.",
                "Blunt Force", "High", "Private Gymnasium",
                "Impact Damage Noted", "Unknown",
                "EVIDENCE SEALED — ACCESS RESTRICTED TO LEAD INVESTIGATOR",
                new List<string>
                {
                    "A cast iron dumbbell of approximately twelve kilograms, recovered from the gymnasium floor adjacent to the weight rack. The impact damage to one end is inconsistent with being dropped — the force vector indicates a deliberate overhead strike.",
                    "The rubber grip coating bears partial pressure impressions. The width of the impressions suggests hands of moderate size — neither the largest nor smallest among the guest list. The gym's security log shows three distinct entries between 10:00 PM and 11:30 PM.",
                    "What is most notable about this piece of evidence is not what it is, but where it was found — placed back on the rack, wrong weight, wrong position, in a manner that suggests familiarity with the space rather than panic. Someone has been here before. Someone knew where things belonged."
                }),

            new("weapon_3",  // Flowerpot
                "Beauty, like danger, is best appreciated from below.",
                "Improvised", "Moderate", "Solarium",
                "Shattered — Partially Reconstructed", "Unknown",
                "EVIDENCE SEALED — ACCESS RESTRICTED TO LEAD INVESTIGATOR",
                new List<string>
                {
                    "A large terracotta flowerpot, originally housing a specimen of Euphorbia milii — crown of thorns — that stood on the upper shelf of the Solarium's eastern display. The pot was shattered into nine major fragments and numerous minor shards, consistent with a fall from height rather than a direct throw.",
                    "The plant itself was found undisturbed, its root ball still intact, suggesting the pot was displaced without disturbing its contents. This implies either a considered removal or a very particular kind of accident. The thorns bore evidence of fabric transfer — a thread that does not match any item in the estate's linen inventory.",
                    "Reconstructed from its fragments, the pot stands thirty-two centimetres tall and weighs approximately four kilograms when empty. The force required to deploy it as an instrument would not have been inconsiderable. The investigator notes that the upper shelf requires a step stool or a person of unusual height to reach comfortably."
                }),

            new("weapon_1",  // Mower
                "Cultivation is merely controlled destruction. I have simply removed the controls.",
                "Garden Equipment", "High", "Garden Shed",
                "Engine Warm at Discovery", "Unknown",
                "EVIDENCE SEALED — ACCESS RESTRICTED TO LEAD INVESTIGATOR",
                new List<string>
                {
                    "A commercial riding mower, property of the Blackwood Estate groundskeeping staff, discovered with its engine still warm at 11:47 PM. The last authorized maintenance was logged two days prior; no scheduled mowing was planned for the evening in question.",
                    "The blade housing shows impact damage to the forward intake guard. Analysis of the recovered material from within the housing is currently underway. The mower's GPS logging unit — a standard feature on this model — was found to have been disabled approximately forty-eight hours before the incident.",
                    "It is worth noting that the route between the garden shed and the swimming pool annex can be covered in under four minutes at the mower's maximum operational speed. The investigators have flagged this route in their spatial analysis of the evening's timeline."
                }),

            new("weapon_4",  // Plastic Bag
                "The most elegant solutions leave nothing behind. Almost nothing.",
                "Suffocation", "High", "Kitchen",
                "Trace Evidence Present", "Unknown",
                "EVIDENCE SEALED — ACCESS RESTRICTED TO LEAD INVESTIGATOR",
                new List<string>
                {
                    "A heavy-gauge clear plastic bag, the kind used for kitchen storage of large items, found folded beneath the third drawer of the butler's pantry. It would have been invisible to a casual inspection. The fold lines suggest it had been deliberately concealed rather than simply left.",
                    "Interior surface analysis revealed trace moisture and microscopic fiber consistent with human hair. The exterior bears a partial fingerprint — smudged, but partially recoverable. The bag is not from the estate's standard supplies; its gauge and dimensions match a commercial brand distributed exclusively to hotel kitchens.",
                    "The significance of the kitchen origin is not lost on investigators. Of all the rooms in the estate, the kitchen is the one place where every guest has a plausible reason to briefly appear. It is also, notably, the room farthest from the security camera coverage installed during last year's renovation."
                }),

            new("weapon_6",  // Rat Poison
                "Patience is its own poison. I prefer something more reliable.",
                "Chemical", "Lethal", "Basement Storage",
                "Partially Used — Quantity Unaccounted For", "Unknown",
                "EVIDENCE SEALED — ACCESS RESTRICTED TO LEAD INVESTIGATOR",
                new List<string>
                {
                    "A commercial rodenticide compound, brodifacoum-based, recovered from the basement storage room behind the wine cellar. The container holds twelve hundred grams; an estimated two hundred grams are unaccounted for. The packaging shows no tampering, suggesting knowledge of the correct procedure for dispensing without detection.",
                    "The compound is odorless, tasteless, and soluble in both oil and alcohol. It acts over a period of three to seven days, making the establishment of a precise timeline considerably more complex. The estate's wine cellar contains fourteen bottles opened on the evening in question.",
                    "When the groundskeeper was asked about the poison's presence, he confirmed it had been ordered six months prior for an infestation in the east wing. He further confirmed that he had told at least three guests about it during the informal afternoon tour of the estate grounds — a detail he now regards with evident regret."
                }),
        };

        foreach (var lore in loreList)
        {
            var existing = await col.Find(w => w.Id == lore.Id).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(existing?.Quote)) continue;

            var filter = Builders<MongoWeapon>.Filter.Eq(w => w.Id, lore.Id);
            var update = Builders<MongoWeapon>.Update
                .Set(w => w.Quote, lore.Quote)
                .Set(w => w.Category, lore.Category)
                .Set(w => w.Lethality, lore.Lethality)
                .Set(w => w.Origin, lore.Origin)
                .Set(w => w.Condition, lore.Condition)
                .Set(w => w.HandledBy, lore.HandledBy)
                .Set(w => w.Evidence, lore.Evidence)
                .Set(w => w.Description, lore.Description);
            await col.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }
    }

    // ── Locations ─────────────────────────────────────────────────────────────
    // IDs from database/locations.json:
    //   location_1 = Cinema       location_6 = Garden
    //   location_2 = Gym          location_7 = Elevator
    //   location_3 = Swimming Pool location_8 = Helipad
    //   location_4 = Basement     location_9 = Panic Room
    //   location_5 = Solarium

    private async Task SeedLocationsAsync()
    {
        var col = _db.GetCollection<MongoLocation>("locations");

        var loreList = new LocationLore[]
        {
            new("location_4",  // Basement
                "Everything of consequence is buried. One simply needs to know where to dig.",
                "Underground Level", "Restricted", "Limited Access",
                "Groundskeeper (authorized), Estate Staff",
                "Water damage — pipes repaired March 2024",
                "Stairwell from kitchen corridor; service elevator (key required)",
                "INVESTIGATOR NOTES — RESTRICTED ACCESS",
                new List<string>
                {
                    "The basement of the Blackwood Estate is a labyrinthine network of storage rooms, utility corridors, and what was once described in the original architect's notes as 'chambers of necessary discretion.' It spans the full footprint of the manor above and includes three distinct sections: the wine cellar, the workshop, and the storage archive.",
                    "Access is technically restricted to authorized staff, though the key distribution records have not been audited in three years. The stairwell from the kitchen corridor is the primary route; the service elevator is nominally locked, though the mechanism is known to respond to the override code that was last changed in 2019.",
                    "Three separate guests visited the basement during the evening, each citing different reasons. Their accounts have been logged. Their timings do not fully align with the security system's entry and exit records, suggesting at least one visit was not logged through official channels."
                }),

            new("location_1",  // Cinema
                "In the dark, everyone is a stranger. That is precisely the point.",
                "Ground Floor West", "Low", "Open Access",
                "Dahlia Onix (observed 9:00 PM — 10:42 PM, 10:57 PM — ?)",
                "Projector malfunction — bulb replaced February 2024",
                "Main corridor double doors; emergency exit (alarmed)",
                "INVESTIGATOR NOTES — RESTRICTED ACCESS",
                new List<string>
                {
                    "The private cinema seats twenty-four and is equipped with a 35mm projector alongside a modern digital system. On the evening of the incident, the digital system was queued with three reels from the estate's private collection: footage dated 1963, 1971, and one reel with no date or label.",
                    "The cinema is the one room in the estate where darkness is not merely a condition but a design principle. There are no windows, no external light sources, and the soundproofing was installed to a specification that the original contractor described as 'well beyond residential requirements.'",
                    "The unlabeled reel has been secured by the investigation. Its content is currently under review. The investigator notes that someone went to considerable effort to ensure it was the last in the queue — a position that guaranteed it would only play if the viewer stayed until the end, or if they already knew what they were looking for."
                }),

            new("location_7",  // Elevator
                "Movement between floors is the movement between worlds. Choose your destination carefully.",
                "All Floors", "Medium", "Monitored",
                "Transit use only — multiple occupants logged",
                "Sensor fault — maintenance completed January 2024",
                "All floor lobbies (requires keycard above ground floor)",
                "INVESTIGATOR NOTES — RESTRICTED ACCESS",
                new List<string>
                {
                    "The estate elevator connects all four levels including the basement and rooftop helipad. Its access log recorded fourteen individual journeys between 8:00 PM and midnight. Of these, three journeys show a weight discrepancy suggesting either a malfunction in the sensors or the presence of a second, unlogged occupant.",
                    "The cabin measures two metres by two metres — intimate by design, as the original specification notes describe it, intended to force brief but meaningful conversation between floors. The mirrored walls have been a point of considerable aesthetic debate; the investigator notes they are ideal for concealing that one is watching without appearing to watch.",
                    "The security footage from the elevator cabin covers a forty-second blind spot at each floor transition, a known technical limitation that was noted in the 2023 safety review and not yet remedied. This forty-second window has become the focal point of no fewer than four separate lines of inquiry."
                }),

            new("location_6",  // Garden
                "Nothing grows without intention. And nothing dies without cause.",
                "Exterior Grounds", "Low", "Open Access",
                "Fern Malachite (east boundary, 11:00 PM — 11:18 PM)",
                "Perimeter fence breach — repair logged October 2023",
                "Garden doors (ground floor, east and west); perimeter gate (staff only)",
                "INVESTIGATOR NOTES — RESTRICTED ACCESS",
                new List<string>
                {
                    "The formal gardens of the Blackwood Estate cover approximately two acres of manicured grounds. The east garden contains the night-blooming collection — jasmine, moonflowers, and evening primrose — which were at peak bloom on the evening of the incident. The fragrance, multiple guests noted, was extraordinary.",
                    "The garden's perimeter lighting operates on a motion-activated circuit with a three-second delay. This delay creates brief windows of darkness along the east boundary — windows that, with knowledge of the timing, could be navigated without triggering the full illumination array. The investigator notes that this knowledge requires either professional expertise or prior reconnaissance.",
                    "Soil analysis along the east boundary path has identified footprint impressions from at least two distinct individuals in addition to the groundskeeper's standard boot pattern. One set of impressions terminates at the hedge line and does not reappear. The hedge in question borders the swimming pool annex."
                }),

            new("location_2",  // Gym
                "Strength is not the measure of force. It is the measure of control.",
                "Ground Floor East", "Medium", "Members Only",
                "Poppy Ruby (claimed 10:15 PM — 11:30 PM)",
                "Mirror panel replacement — March 2024",
                "East corridor door (keycard); emergency stair access (alarmed)",
                "INVESTIGATOR NOTES — RESTRICTED ACCESS",
                new List<string>
                {
                    "The private gymnasium is equipped to professional standards: a full rack of free weights, three treadmills, a boxing station, and floor-to-ceiling mirrors along the north wall. The replacement of the north mirror panel — noted in the maintenance log from March — involved a crack that the contractor's report described as 'impact-related' rather than structural.",
                    "The gym's security log shows Poppy Ruby's keycard entry at 10:15 PM. The next logged entry is her exit at 11:32 PM. In the intervening hour and seventeen minutes, no other keycard entries were logged. The equipment telemetry — available on all networked machines — shows zero active sessions during this period.",
                    "The boxing station bears marks consistent with heavy use. The bag's suspension chain shows micro-fractures at the uppermost link — the kind of fractures associated with repeated high-force impacts over time, not a single session. Someone has been using this gym more often than the records suggest. Possibly much more often."
                }),

            new("location_8",  // Helipad
                "Elevation grants perspective. It also grants anonymity.",
                "Rooftop", "High", "Restricted",
                "Gardenia Quartz (arrival, time unlogged)",
                "Navigation light fault — November 2023",
                "Rooftop stairwell (double-keycard required); helicopter landing (external)",
                "INVESTIGATOR NOTES — RESTRICTED ACCESS",
                new List<string>
                {
                    "The helipad occupies the full eastern third of the rooftop and is rated for rotorcraft up to 4,500 kilograms. It is not visible from the estate's ground-floor windows or the garden due to the building's parapet design — a feature that the original architect apparently considered both aesthetic and deliberate.",
                    "Access requires two keycards to be presented simultaneously at the rooftop stairwell — a security measure implemented after an incident in 2021 that the estate's records describe only as 'unauthorized access, external party.' Both keycards are logged; the log for the evening of the incident shows a single-card access attempt at 9:23 PM that was denied, followed by a successful dual-card access at 9:31 PM.",
                    "The wind at rooftop level on the evening of the incident was measured at seventeen knots from the northwest. The investigator notes that a conversation held on the helipad under these conditions would be entirely inaudible from any interior location, including the stairwell immediately below. The rooftop is, in the relevant sense, the most private space on the estate."
                }),

            new("location_9",  // Panic Room
                "Safety is an illusion. But some illusions are more useful than others.",
                "Sub-Ground Level", "Maximum", "Emergency Access",
                "Access log encrypted — under warrant review",
                "Biometric reset — February 2024",
                "Concealed entrance (study floor panel); requires biometric + code",
                "INVESTIGATOR NOTES — RESTRICTED ACCESS",
                new List<string>
                {
                    "The panic room exists on no public floor plan of the Blackwood Estate. Its construction was commissioned privately in 2018 and documented in a separate set of architectural drawings held by the estate's legal trust. Three people are believed to have known of its existence prior to the incident. The investigation has since confirmed at least five.",
                    "The room is equipped with seventy-two hours of emergency provisions, an independent air supply, encrypted communication terminals, and a biometric access log that records entry and exit with a time-stamped fingerprint scan. The log for the relevant period has been secured under a legal warrant. Its contents have not yet been disclosed to the full investigative board.",
                    "The concealed entrance — a floor panel in the study, activated by a specific pressure sequence on the adjacent bookcase — was found in the closed position on the night of the incident. Whether it had been used and re-secured, or never accessed at all, is a question the biometric log will answer. The investigation board is waiting."
                }),

            new("location_5",  // Solarium
                "Light reveals. For those who prefer shadow, a solarium is a form of punishment.",
                "Ground Floor", "Low", "Open Access",
                "Chrysant Pyrite (extended stay, 8:30 PM — 10:00 PM)",
                "Glass panel replacement — January 2024",
                "Drawing room doors (east); garden doors (south)",
                "INVESTIGATOR NOTES — RESTRICTED ACCESS",
                new List<string>
                {
                    "The solarium is the estate's most luminous room, constructed almost entirely of glass on three sides with a vaulted glass ceiling above. During daylight hours it is the social centrepiece of the estate; in the evening it becomes something different — a room where everything inside is visible from without, and everything without is invisible from within.",
                    "The orchid collection that occupies the south display shelves is catalogued and maintained by an outside specialist who visits quarterly. The most recent visit was three weeks prior to the incident. On that visit, a new specimen was added to the upper shelf — the same shelf from which a flowerpot was subsequently dislodged.",
                    "The glass panel replaced in January was broken by an impact from the interior. The official report cites 'thermal stress.' The glazier who performed the replacement noted, in a detail absent from the official report, that the fracture pattern was inconsistent with thermal origin and more consistent with a projected object. This detail has been forwarded to the investigation."
                }),

            new("location_3",  // Swimming Pool
                "Still water is the most deceptive. It reflects everything and reveals nothing.",
                "Ground Floor Annex", "Medium", "Guest Access",
                "Hyacinth Sapphire (poolside, 11:15 PM — confirmed)",
                "Chemical imbalance — corrected April 2024",
                "Annex corridor (ground floor east); garden gate (south side)",
                "INVESTIGATOR NOTES — RESTRICTED ACCESS",
                new List<string>
                {
                    "The swimming pool annex is a self-contained wing attached to the main estate via a glass-walled corridor. The pool itself is heated, covered at night by an automated retractable roof, and illuminated by underwater lighting that casts a distinctive aquamarine reflection visible from the east garden boundary after dark.",
                    "The chemical imbalance recorded in April — a significant chlorine overdose attributed to equipment malfunction — was resolved by the estate's maintenance company. A secondary review of the maintenance logs, conducted as part of the current investigation, has identified an anomaly in the chemical dosing records that does not align with the equipment failure timeline.",
                    "The poolside area is fitted with teak furniture and a small service bar stocked for guests. Three glasses were found rinsed and replaced on the rack on the morning following the incident. The rinsing removed fingerprint evidence. Whether this was courtesy, habit, or deliberate obliteration of evidence is a distinction the investigation is working to establish."
                }),
        };

        foreach (var lore in loreList)
        {
            var existing = await col.Find(l => l.Id == lore.Id).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(existing?.Quote)) continue;

            var filter = Builders<MongoLocation>.Filter.Eq(l => l.Id, lore.Id);
            var update = Builders<MongoLocation>.Update
                .Set(l => l.Quote, lore.Quote)
                .Set(l => l.Floor, lore.Floor)
                .Set(l => l.SecurityLevel, lore.SecurityLevel)
                .Set(l => l.OccupancyStatus, lore.OccupancyStatus)
                .Set(l => l.KnownOccupants, lore.KnownOccupants)
                .Set(l => l.LastIncident, lore.LastIncident)
                .Set(l => l.AccessPoints, lore.AccessPoints)
                .Set(l => l.Notes, lore.Notes)
                .Set(l => l.Description, lore.Description);
            await col.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }
    }
}
